//AJOUTE UN NOUVEAU JOUEUR
exports.Nouveau_Joueur = function (name, color, db, callback) {
    /*var array = {
    ID: '',
    Code: ''
    };*/

    Get_Classement_Joueur_by_elo(1000, db, function (classement) {

        var date = getDateTime();
        var code = Generation_code_client();
        //AJOUTER RANK
        db.query(
        "INSERT INTO players (date_creation_player,nom_joueur,side_red, code, rank, elo)" +
        "VALUES(?,?,?,?,?,?) ",
        [date, name, color, code, classement,1000],
        function (err, result) {
            if (err) callback('ERREUR');

            callback(result.insertId.toString() + ' ' + code.toString());
        });
    });
};

//RECUPERE DATA DE MAINMENU
exports.Recuperation_Data_Main_Menu = function (id_joueur, name, db, callback) {
    /*var array={
    red_win : '',
    green_win : '',
    game_player : '',
    win_player : '',
    elo_player : '',
    classement_player : ''
    };*/
    Verification_Name(id_joueur, name, db, function (blbl) {
        if (blbl == 'true') {
            db.query(
            "SELECT game_play,game_win, elo,rank FROM players WHERE id=?",
            [id_joueur],
            function (err, rows, fields) {
                if (err) callback('ERREUR');

                db.query(
                "SELECT nombre_win_red_total,nombre_win_green_total FROM info_general ORDER BY date DESC LIMIT 1",
                function (err, row) {
                    if (err) callback('ERREUR');
                    if (row[0].nombre_win_red_total == undefined || row[0].nombre_win_red_total == null) {
                        callback(0 + " " + 0 + " " + rows[0].game_play + " " + rows[0].game_win + " " + rows[0].elo + " " + rows[0].rank);
                    }
                    else {
                        callback(row[0].nombre_win_red_total + " " + row[0].nombre_win_green_total + " " + rows[0].game_play + " " + rows[0].game_win + " " + rows[0].elo + " " + rows[0].rank);
                    }
                });
            });
        }
        else {
            callback('false');
        }
    });
};


//VERIFIE SI NOM DEJA PRIS
exports.Verification_Name_Already_Taken = function (name, db, callback) {
    if (db != null) {
        db.query(
    "SELECT id FROM players WHERE nom_joueur =?",
    [name],
    function (err, results) {
        if (err) {
            callback('ERREUR');
        }

        if (results[0] == null || results == "") {
            callback('false');
        }
        else {
            callback('true');
        }
    });
    }
    else {
        callback('ERREUR');
    }
};

var Verification_Name = function (id_joueur, name, db, callback) {
    db.query(
    "SELECT nom_joueur FROM players WHERE id =?",
    [id_joueur],
    function (err, results) {
        if (results[0] == undefined || results[0] == null || results[0].nom_joueur != name) {
            callback('false');
        }
        else {
            callback('true');
        }
    });
}

var CreationPartie = function(id_1, id_2, db)
{
    var date = getDateTime();

    db.query(
    "INSERT INTO  game_attente (date_creation_game,id_player_1,id_player_2)"+
    "VALUES(?,?,?) ",
   [date, id_1, id_2 ],
   function (err) {
    });
}

exports.Creation_Partie = function (room, db) {
    while (room.length > 1) {
        CreationPartie(room[0], room[1], db);
        room.shift();
        room.shift();
    }
};

exports.Client_Verifier_Partie_Trouver = function (id_joueur, db, callback) {
    db.query("SELECT id, id_player_1,id_player_2 FROM game_attente WHERE id_player_1 = ? OR id_player_2 = ?",
    [id_joueur, id_joueur],
    function (err, results) {
        if (err) callback('ERREUR');

        if (results == null || results == "") {
            callback('false');
        }
        else {
            callback('true ' + results[0].id);
        }
    });
}

exports.Client_Verification_Validation_Partie = function (id_client, id_partie, choix, db, callback) {
    /*var array = {
    result: '', // 0 =nope , 1 = attente_adversaire , 2 = oui, 3 = refuse, 4 = out of time
    id_partie: '',
    id_adversaire: '',
    nom_adversaire: '',
    classement_adversaire: '',
    elo_adversaire: ''
    };*/

    db.query("SELECT id_player_1,id_player_2,player_1_ok,player_2_ok, date_creation_game FROM game_attente WHERE id=?",
    [id_partie],
    function (err, results) {
        if (err) callback('ERREUR');

        if (results == null || results == "") {
            callback('ERREUR');
        }
        else {
            if (id_client == results[0].id_player_1) {
                if (choix == '0') {
                    db.query("UPDATE game_attente SET player_1_ok=2 WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        callback('ok');
                    });
                }

                else if (results[0].player_2_ok == "2") {
                    db.query("DELETE FROM game_attente WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        callback('3');
                    });
                }
                else if (getDifferenceTemps_en_secondes(results[0].date_creation_game) > 10) {
                    db.query("DELETE FROM game_attente WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        callback('4');
                    });
                }
                else if (results[0].player_2_ok == "1" && results[0].player_1_ok == "0") {
                    Passer_Partie_En_Initialization(results[0].id_player_1, results[0].id_player_2, db, function (id_partie_final) {
                        db.query("SELECT id_player_1, id_player_2,name_player_2,rank_player_2_debut,elo_player_2_debut, plateau FROM game WHERE id=?",
                            [id_partie_final],
                            function (err, results_bis) {
                                if (err) callback('ERREUR');

                                db.query("UPDATE game_attente SET id_partie_final=?,player_1_ok=1 WHERE id=?",
                                [id_partie_final, id_partie],
                                function (err) {
                                    if (err) callback('ERREUR');

                                    callback('2 ' + id_partie_final + ' ' + results_bis[0].id_player_2 + ' ' + results_bis[0].name_player_2 + ' ' + results_bis[0].rank_player_2_debut + ' ' + results_bis[0].elo_player_2_debut + ' ' + results_bis[0].plateau + ' ' + results_bis[0].id_player_1);
                                });
                            });
                    });
                }
                else if (results[0].player_1_ok == "1" && results[0].player_2_ok == "1") {
                    db.query("SELECT id_partie_final FROM game_attente WHERE id=?",
                        [id_partie],
                        function (err, results_2) {
                            if (err) callback('ERREUR');

                            db.query("SELECT id_player_1,id_player_2,name_player_2,rank_player_2_debut,elo_player_2_debut, plateau FROM game WHERE id=?",
                            [results_2[0].id_partie_final],
                            function (err, results_3) {
                                if (err) callback('ERREUR');

                                callback('2 ' + results_2[0].id_partie_final + ' ' + results_3[0].id_player_2 + ' ' + results_3[0].name_player_2 + ' ' + results_3[0].rank_player_2_debut + ' ' + results_3[0].elo_player_2_debut + " " + results_3[0].plateau+ ' ' + results_3[0].id_player_1);
                            });
                        });
                }
                else {
                    db.query("UPDATE game_attente SET player_1_ok=1 WHERE id=?",
                        [id_partie],
                        function (err) {
                            if (err) callback('ERREUR');
                            callback('1');
                        });
                }
            }
            else {
                if (choix == '0') {
                    db.query("UPDATE game_attente SET player_2_ok=2 WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        callback('ok');
                    });
                }
                else if (results[0].player_1_ok == "2") {
                    db.query("DELETE FROM game_attente WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        callback('3');
                    });
                }
                else if (getDifferenceTemps_en_secondes(results[0].date_creation_game) > 10) {
                    db.query("DELETE FROM game_attente WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        callback('4');
                    });
                }
                else if (results[0].player_1_ok == "1" && results[0].player_2_ok == "0") {
                    Passer_Partie_En_Initialization(results[0].id_player_1, results[0].id_player_2, db, function (id_partie_final) {

                        db.query("SELECT id_player_1,name_player_1,rank_player_1_debut,elo_player_1_debut, plateau FROM game WHERE id=?",
                            [id_partie_final],
                            function (err, results_bis) {
                                if (err) callback('ERREUR');

                                db.query("UPDATE game_attente SET id_partie_final=?,player_2_ok=1 WHERE id=?",
                                [id_partie_final, id_partie],
                                function (err) {
                                    if (err) callback('ERREUR');

                                    callback('2 ' + id_partie_final + ' ' + results_bis[0].id_player_1 + ' ' + results_bis[0].name_player_1 + ' ' + results_bis[0].rank_player_1_debut + ' ' + results_bis[0].elo_player_1_debut + " " + results_bis[0].plateau+ ' ' + results_bis[0].id_player_1);
                                });
                            });
                    });
                }
                else if (results[0].player_1_ok == "1" && results[0].player_2_ok == "1") {
                    db.query("SELECT id_partie_final FROM game_attente WHERE id=?",
                        [id_partie],
                        function (err, results_2) {
                            if (err) callback('ERREUR');

                            db.query("SELECT id_player_1,name_player_1,rank_player_1_debut,elo_player_1_debut, plateau FROM game WHERE id=?",
                            [results_2[0].id_partie_final],
                            function (err, results_3) {
                                if (err) callback('ERREUR');

                                callback('2 ' + results_2[0].id_partie_final + ' ' + results_3[0].id_player_1 + ' ' + results_3[0].name_player_1 + ' ' + results_3[0].rank_player_1_debut + ' ' + results_3[0].elo_player_1_debut + " " + results_3[0].plateau+ ' ' + results_3[0].id_player_1);
                            });
                        });
                }
                else {
                    db.query("UPDATE game_attente SET player_2_ok=1 WHERE id=?",
                        [id_partie],
                        function (err) {
                            if (err) callback('ERREUR');
                            callback('1');
                        });
                }
            }
        }
    });
};

var Passer_Partie_En_Initialization = function (id_joueur_1, id_joueur_2, db, callback) {
    var date = getDateTime();

    var elo_1;
    var elo_2;
    var rank_1;
    var rank_2;
    var name_1;
    var name_2;
    var id_partie_final;

    db.query(
    "SELECT elo, rank, nom_joueur FROM players WHERE id=?",
    [id_joueur_1],
     function (err, result) {
         if (err) callback('ERREUR');

         db.query(
        "SELECT elo, rank ,nom_joueur FROM players WHERE id=?",
        [id_joueur_2],
        function (err, results) {
            if (err) callback('ERREUR');

            var plateau = Plateau_Initialization();

            db.query(
            "INSERT INTO game (date_creation_game,id_player_1,id_player_2,name_player_1,name_player_2,rank_player_1_debut,rank_player_2_debut,elo_player_1_debut,elo_player_2_debut, plateau)" +
            "VALUES(?,?,?,?,?,?,?,?,?,?) ",
            [date, id_joueur_1, id_joueur_2, result[0].nom_joueur, results[0].nom_joueur, result[0].rank, results[0].rank, result[0].elo, results[0].elo, plateau],
            function (err, insert_result) {
                if (err) callback('ERREUR');
                Set_next_play_before_this_time(db, insert_result.insertId.toString(), function () {
                    Set_last_co(db, insert_result.insertId.toString(), id_joueur_1, function () {
                        Set_last_co(db, insert_result.insertId.toString(), id_joueur_2, function () {
                            callback(insert_result.insertId.toString());
                        });
                    });
                });
            });
        });
     });
}

exports.Delete_Parties_en_Attente = function (db) {
    db.query(
    "SELECT id,date_creation_game FROM game_attente",
     function (err, results) {
         for (var i in results) {

             if (getDifferenceTemps_en_secondes(results[i].date_creation_game) > 15) {
                 db.query(
                "DELETE FROM game_attente WHERE id=?",
                [results[i].id],
                function (err) {

                });
             }
         }
     });
};

exports.Client_Passe_Partie_En_Pret = function (id_partie, id_client, db, callback) {
    /*var array = {
    result: '',
    statut_partie: ''
    }*/
    Set_last_co(db, id_partie, id_client, function () {
        db.query(
    "SELECT id_player_1 FROM game WHERE id=?",
    [id_partie],
     function (err, results) {
         if (err) callback('ERREUR');
         if (results[0].id_player_1 == id_client) {
             db.query(
            "SELECT players_2_pret,players_1_pret FROM game WHERE id=?",
            [id_partie],
            function (err, results_1) {
                if (err) callback('ERREUR');

                if (results_1[0].players_2_pret == '1' && results_1[0].players_1_pret == '1') {
                    db.query(
                    "SELECT statut_partie, next_play_before_this_time FROM game WHERE id=?",
                    [id_partie],
                    function (err, rows) {
                        if (err) callback('ERREUR');
                        callback('true ' + rows[0].statut_partie + ' ' + rows[0].next_play_before_this_time);
                    });
                }
                else if (results_1[0].players_2_pret == '1') {
                    db.query(
                    "UPDATE game SET players_1_pret=1 WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        var rand = RandomQuiJoueEnPremier();
                        if (rand) {

                            db.query(
                            "UPDATE game SET statut_partie=3 WHERE id=?",
                            [id_partie],
                            function (err) {
                                if (err) callback('ERREUR');
                                Set_next_play_before_this_time(db, id_partie, function (info) {
                                    callback('true 3 ' + info);
                                });
                            });
                        }
                        else {
                            db.query(
                            "UPDATE game SET statut_partie=4 WHERE id=?",
                            [id_partie],
                            function (err) {
                                if (err) callback('ERREUR');
                                Set_next_play_before_this_time(db, id_partie, function (info) {
                                    callback('true 4 ' + info);
                                });
                            });
                        }
                    });
                }
                else {
                    db.query(
                    "UPDATE game SET players_1_pret=1 WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        callback('false 1');

                    });
                }
            });
         }
         else {
             db.query(
            "SELECT players_1_pret, players_2_pret FROM game WHERE id=?",
            [id_partie],
            function (err, results_1) {
                if (err) callback('ERREUR');
                if (results_1[0].players_2_pret == '1' && results_1[0].players_1_pret == '1') {
                    db.query(
                    "SELECT statut_partie,next_play_before_this_time  FROM game WHERE id=?",
                    [id_partie],
                    function (err, rows) {
                        if (err) callback('ERREUR');
                        callback('true ' + rows[0].statut_partie + ' ' + rows[0].next_play_before_this_time);
                    });
                }
                else if (results_1[0].players_1_pret == "1") {
                    db.query(
                    "UPDATE game SET players_2_pret=1 WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        var rand = RandomQuiJoueEnPremier();
                        if (rand) {

                            db.query(
                            "UPDATE game SET statut_partie=4 WHERE id=?",
                            [id_partie],
                            function (err) {
                                if (err) callback('ERREUR');
                                Set_next_play_before_this_time(db, id_partie, function (info) {
                                    callback('true 4 ' + info);
                                });
                            });
                        }
                        else {
                            db.query(
                            "UPDATE game SET statut_partie=3 WHERE id=?",
                            [id_partie],
                            function (err) {
                                if (err) callback('ERREUR');
                                Set_next_play_before_this_time(db, id_partie, function (info) {
                                    callback('true 3 ' + info);
                                });

                            });
                        }
                    });
                }
                else {
                    db.query(
                    "UPDATE game SET players_2_pret=1 WHERE id=?",
                    [id_partie],
                    function (err) {
                        if (err) callback('ERREUR');
                        callback('false 1');
                    });
                }
            });
         }
     });
    });
};

exports.Verification_Temps_Depassement = function (db) {
    var newTime = new Date();
    var minutes = newTime.getMinutes().toString();
    var secondes = newTime.getSeconds().toString();
    var caca = minutes + ":" + secondes;

    db.query(
    "SELECT next_play_before_this_time, statut_partie, id_player_1, id_player_2, id, historique_game FROM game WHERE statut_partie=3 OR statut_partie=4",
     function (err, results) {
         if (err) callback('ERREUR');
         for (var i in results) {
             if (results[i].next_play_before_this_time != null && results[i].next_play_before_this_time != undefined && Calcul_Depassement_Temps(results[i].next_play_before_this_time, 20)) {
                 //UN DEPASSEMENT DE TEMPS A ETE AVERE, RAPE TIME 
                 Set_next_play_before_this_time(db, results[i].id, function () {
                     var id_player;
                     if (results[i].statut_partie == "3") {
                         id_player = results[i].id_player_1;
                         var blbl = AjoutCoupString(results[i].historique_game, id_player, 2, -1, -1, -1);
                         db.query(
                         "UPDATE game SET statut_partie=4, historique_game=? WHERE id=?",
                         [blbl, results[i].id],
                         function (err) {
                             if (err) callback('ERREUR');

                         });
                     }
                     else {
                         id_player = results[i].id_player_2;
                         var blbl = AjoutCoupString(results[i].historique_game, id_player, 2, -1, -1, -1);
                         db.query(
                    "UPDATE game SET statut_partie=3, historique_game=?  WHERE id=?",
                    [blbl, results[i].id],
                    function (err) {
                        if (err) callback('ERREUR');

                    });
                     }
                 });
             }
         }

     });
};

var Calcul_Depassement_Temps = function (time, temps_sec_max) // FORMAT HH:SS
{
    var time_max = parseInt(temps_sec_max);
    if (time == null || time == undefined) {
        return false;
    }
    else {
        var blbl = time.split(":");
        var minutes = parseInt(blbl[0]);
        var secondes = parseInt(blbl[1]);

        var temps = new Date();
        var temps_min = temps.getMinutes();
        var temps_sec_now = temps.getSeconds();

        var total_secondes_next_time_before = minutes * 60 + secondes;
        var total_temps_sec_now = temps_min * 60 + temps_sec_now;

        if (minutes == 59 && secondes > 57) {
            if (total_temps_sec_now > total_secondes_next_time_before || total_temps_sec_now < 60) {
                return true;
            }
            else {
                return false;
            }
        }
        else if (minutes == 0 && secondes < time_max) {
            if (total_temps_sec_now < 60 && total_temps_sec_now > secondes) {
                return true;
            }
            else {
                return false;
            }
        }
        else if (total_temps_sec_now > total_secondes_next_time_before) {
            return true;
        }
        else {
            return false;
        }
    }
}

var Set_next_play_before_this_time = function (db, id_partie, callback) {
    var newTime = new Date();
    newTime.setSeconds(newTime.getSeconds() + 20);
    var minutes = newTime.getMinutes().toString();
    var secondes = newTime.getSeconds().toString();
    var caca = minutes + ":" + secondes;

    db.query(
     "UPDATE game SET next_play_before_this_time=? WHERE id=?",
     [caca, id_partie],
     function (err) {
         if (err) callback('ERREUR');
         callback(caca);
     });
}

exports.Get_Next_time_before_this_time = function (db, id_partie, callback) {
    db.query(
    "SELECT next_play_before_this_time FROM game WHERE id=?",
    [id_partie],
     function (err, results) {
         callback(results[0].next_play_before_this_time);
     });
};

var Set_last_co = function (db, id_partie, id_joueur, callback) {
    var newTime = new Date();
    newTime.setSeconds(newTime.getSeconds() + 30);
    var minutes = newTime.getMinutes().toString();
    var secondes = newTime.getSeconds().toString();
    var caca = minutes + ":" + secondes;

    db.query(
    "SELECT  id_player_2 FROM game WHERE id=?",
    [id_partie],
     function (err, results) {

         if (id_joueur == results[0].id_player_2) {
             db.query(
            "UPDATE game SET last_co_player_2=? WHERE id=?",
            [caca, id_partie],
            function (err) {
                if (err) callback('ERREUR');
                callback();
            });
         }
         else {
             db.query(
            "UPDATE game SET last_co_player_1=? WHERE id=?",
            [caca, id_partie],
            function (err) {
                if (err) callback('ERREUR');
                callback();
            });
         }
     });
}

var Get_minute_seconde_from_date = function () {
    var newTime = new Date();
    var minutes = newTime.getMinutes().toString();
    var secondes = newTime.getSeconds().toString();
    var caca = minutes + ":" + secondes;
    return caca;
}

exports.Client_Recuperer_Temps_Partie = function (id_partie, id_joueur, db, callback) {
    Set_last_co(db, id_partie, id_joueur, function () {
        db.query(
        "SELECT next_play_before_this_time FROM game WHERE id=?",
        [id_partie],
        function (err, results) {
            if (err) callback('ERREUR');
            var temps = Temps_Restant(results[0].next_play_before_this_time);
            callback(temps);
        });
    });
};

exports.Client_Abandonner_Partie = function (id_partie, id_joueur, db, callback) {
    db.query(
    "SELECT plateau, id_player_1 FROM game WHERE id=?",
    [id_partie],
    function (err, results) {
        if (err) callback('ERREUR');

        var id_player_win;
        if (id_joueur == results[0].id_player_1) {
            id_player_win = 4;
        }
        else {
            id_player_win = 3;
        }

        Remplissage_Plateau(results[0].plateau, id_player_win, function (plateau_callback) {
            Determine_Result_Game(plateau_callback, function (statut_partie_player_1) {
                Fin_de_Partie(id_partie, statut_partie_player_1, plateau_callback, db, function () {
                    callback('true');
                });
            });
        });
    });
};

exports.Defaite_Apres_Absence_Prolonge = function (db) {
    db.query(
    "SELECT last_co_player_1, last_co_player_2, id, id_player_1,id_player_2 FROM game WHERE statut_partie = 3 OR statut_partie = 4",
    function (err, results) {
        for (var i in results) {
            if (results[i].id != null || results[i].id != undefined || results[i].last_co_player_1 != null || results[i].last_co_player_1 != undefined) {
                if (Calcul_Depassement_Temps(results[i].last_co_player_1, 30)) {
                    exports.Client_Abandonner_Partie(results[i].id, results[i].id_player_1, db, function (blbl) {

                    });
                }
                else if (Calcul_Depassement_Temps(results[i].last_co_player_2, 30)) {
                    exports.Client_Abandonner_Partie(results[i].id, results[i].id_player_2, db, function (blbl) {

                    });
                }
            }
        }
    });
};

exports.Client_Recuperer_Info_Game_En_Cours = function (id_partie, id_joueur, db, callback) {
    /* return
    result,
    statut_partie,
    plateau,
    time_ecoule_coup*/

    Set_last_co(db, id_partie, id_joueur, function () {

        db.query(
        "SELECT plateau, statut_partie, next_play_before_this_time , id_player_1 FROM game WHERE id=?",
        [id_partie],
        function (err, results) {
            if (err) callback('ERREUR');
            if (results[0].statut_partie != '2') {
                var temps = Temps_Restant(results[0].next_play_before_this_time);
                callback('true ' + results[0].id_player_1 + ' ' + results[0].statut_partie + ' ' + results[0].plateau + ' ' + results[0].next_play_before_this_time + ' ' + temps);
            }
            else {
                Client_Recuperation_Info_Fin_Partie(id_joueur, id_partie, db, function (caca) {
                    var info_fin_partie = caca[0] + "_" + caca[1] + "_" + caca[2] + "_" + caca[3] + "_" + caca[4] + "_" + caca[5];
                    callback('true ' + results[0].id_player_1 + ' ' + results[0].statut_partie + ' ' + results[0].plateau + ' ' + results[0].next_play_before_this_time + ' ' + info_fin_partie);
                });
            }
        });
    });
};

exports.Client_Coup_Jouer_Joueur = function (id_partie, id_joueur, detail_coup, db, callback) {
    /*var array = {
    result: '',
    statut_partie: '', // 3 = Tour de joueur n1, 4 = Tour de joueur n2, 2 = fin_de_partie, 1= en_attente_debut
    info_fin_partie: '',
    plateau: '',
    detail_coup_jouer: ''
    }*/

    var id_plateau_joueur; //(3 ou 4)
    var id_plateau_adversaire;
    var coup = detail_coup.split("_"); // 0 = type_coup (0 = evol, 1= saut, 2= pas_assez_temps) , 1= index_origin, 2= index

    Set_last_co(db, id_partie, id_joueur, function () {


        db.query(
    "SELECT id_player_1, plateau, statut_partie, historique_game, plateau FROM game WHERE id=?",
    [id_partie],
     function (err, results) {
         if (err) callback('ERREUR');

         if (id_joueur == results[0].id_player_1) {
             id_plateau_joueur = 3;
             id_plateau_adversaire = 4;
         }
         else {
             id_plateau_joueur = 4;
             id_plateau_adversaire = 3;
         }

         if (Verification_Case_Vide_Et_Possible(coup[2], coup[1], results[0].plateau)) {
             var new_plateau = ReplaceAt(results[0].plateau, coup[2], id_plateau_joueur.toString());
             if (coup[0] == "1") {
                 new_plateau = ReplaceAt(new_plateau, coup[1], "0");
             }

             Contamination(coup[2], id_plateau_joueur, new_plateau, function (data) {
                 new_plateau = data[0];
                 var index_contamination = "";

                 for (var i = 0; i < data[1].length; i++) {
                     if (i != 0) {
                         index_contamination = index_contamination + "?" + data[1][i].toString();
                     }
                     else {
                         index_contamination = data[1][i].toString();
                     }
                 }

                 var detail_coup_jouer = id_joueur + '_' + detail_coup + '_' + Get_minute_seconde_from_date() + '_' + index_contamination;

                 if (Verification_Possibilite_Jouer(id_plateau_adversaire, new_plateau)) {
                     if (id_plateau_joueur == 3) {
                         DB_Update_Coup_Joueur(db, 4, id_partie, id_joueur, results[0].historique_game, coup, new_plateau, index_contamination, function () {
                             callback('true 4 -1 ' + new_plateau + ' ' + detail_coup_jouer);
                         });
                     }
                     else {
                         DB_Update_Coup_Joueur(db, 3, id_partie, id_joueur, results[0].historique_game, coup, new_plateau, index_contamination, function () {
                             callback('true 3 -1 ' + new_plateau + ' ' + detail_coup_jouer);
                         });
                     }
                 }
                 else {
                     DB_Update_Coup_Joueur(db, 2, id_partie, id_joueur, results[0].historique_game, coup, new_plateau, index_contamination, function () {
                         Remplissage_Plateau(new_plateau, id_plateau_joueur, function (plateau_callback) {
                             Determine_Result_Game(plateau_callback, function (statut_partie_player_1) {
                                 Fin_de_Partie(id_partie, statut_partie_player_1, plateau_callback, db, function () {
                                     Client_Recuperation_Info_Fin_Partie(id_joueur, id_partie, db, function (caca) {
                                         var info_fin_partie = caca[0] + "_" + caca[1] + "_" + caca[2] + "_" + caca[3] + "_" + caca[4] + "_" + caca[5];
                                         callback('true 2 ' + info_fin_partie + ' ' + plateau_callback + ' ' + detail_coup_jouer);
                                     });
                                 });
                             });
                         });
                     });
                 }
             });
         }
         else {
             callback('false');
         }
     });
    });
};

var Determine_Result_Game = function (plateau, callback) {

    var caca = Recuperation_Nombre_Point_Plateau(plateau);
    if (caca[0] > caca[1]) {
        callback(1);
    }
    else if (caca[0] == caca[1]) {
        callback(2);
    }
    else {
        callback(3);
    }
}

var DB_Update_Coup_Joueur = function (db, statut_partie, id_partie, id_joueur, historique, coup_detail, plateau, contamination_index, callback) {
    Set_next_play_before_this_time(db, id_partie, function () {
        var new_historique = AjoutCoupString(historique, id_joueur, coup_detail[0], coup_detail[1], coup_detail[2], contamination_index);
        var point = Recuperation_Nombre_Point_Plateau(plateau);
        db.query(
        "UPDATE game SET statut_partie=?, historique_game=?, point_player_1= ?, point_player_2 = ?, plateau = ? WHERE id=?",
        [statut_partie, new_historique, point[0], point[1], plateau, id_partie],
        function (err) {
            if (err) callback('ERREUR');
            callback();
        });
    });
}

exports.Client_Verification_Si_Adversaire_A_Jouer = function (id_partie, id_joueur, db, callback) {
    /*var array = {
    result: '',
    plateau: '',
    detail_coup_jouer: '',
    statut_partie: '',
    info_fin_partie: ''
    }*/

    var adversaire_a_jouer;

    Set_last_co(db, id_partie, id_joueur, function () {

        db.query(
    "SELECT plateau, historique_game, statut_partie, next_play_before_this_time FROM game WHERE id=?",
    [id_partie],
     function (err, results) {
         if (err) callback('ERREUR');

         if (Verification_Adversaire_Jouer(id_joueur, id_partie, results[0].historique_game, db) == false) {
             var temps = Temps_Restant(results[0].next_play_before_this_time);
             callback('false ' + results[0].plateau + ' ' + temps);
         }
         else {
             var detail_coup_jouer = Client_Recuperation_Data_Dernier_Coup(results[0].historique_game);
             if (parseInt(results[0].statut_partie) == 2) {
                 Client_Recuperation_Info_Fin_Partie(id_joueur, id_partie, db, function (caca) {
                     var info_fin_partie = caca[0] + "_" + caca[1] + "_" + caca[2] + "_" + caca[3] + "_" + caca[4] + "_" + caca[5];
                     callback('true ' + results[0].plateau + ' ' + detail_coup_jouer + ' ' + results[0].statut_partie + ' ' + info_fin_partie);
                 });
             }
             else {
                 callback('true ' + results[0].plateau + ' ' + detail_coup_jouer + ' ' + results[0].statut_partie + ' -1');
             }
         }
     });
    });
};

var Fin_de_Partie = function (id_partie, statut_partie_joueur_1, plateau, db, callback) {   // statut_partie -> 1win 2 draw 3 loose
    var statut_partie_joueur_2 = 0;
    if (statut_partie_joueur_1 == '1') {
        statut_partie_joueur_2 = 3;
        Db_Update_Fin_Partie(db, statut_partie_joueur_1, statut_partie_joueur_2, plateau, id_partie, function () {
            callback();
        });
    }
    else if (statut_partie_joueur_1 == '2') {
        statut_partie_joueur_2 = 2;
        Db_Update_Fin_Partie(db, statut_partie_joueur_1, statut_partie_joueur_2, plateau, id_partie, function () {
            callback();
        });
    }
    else {
        statut_partie_joueur_2 = 1;
        Db_Update_Fin_Partie(db, statut_partie_joueur_1, statut_partie_joueur_2, plateau, id_partie, function () {
            callback();
        });
    }
}

var Db_Update_Fin_Partie = function (db, statut_partie_joueur_1, statut_partie_joueur_2, plateau, id_partie, callback) {

    db.query(
        "SELECT id_player_1,id_player_2,elo_player_1_debut, elo_player_2_debut FROM game WHERE id=?",
        [id_partie],
        function (err, results) {
            if (err) callback('ERREUR');

            Calcule_Nouveau_Elo(results[0].elo_player_1_debut, results[0].elo_player_2_debut, statut_partie_joueur_1, function (new_elo_1) {
                Calcule_Nouveau_Elo(results[0].elo_player_2_debut, results[0].elo_player_1_debut, statut_partie_joueur_2, function (new_elo_2) {
                    Get_Classement_Joueur_by_elo(new_elo_1, db, function (rank_1) {
                        Get_Classement_Joueur_by_elo(new_elo_2, db, function (rank_2) {

                            var point = Recuperation_Nombre_Point_Plateau(plateau);

                            db.query(
                            "UPDATE game SET statut_partie = 2 , date_fin_game=?,plateau=?, point_player_1=?, point_player_2 =?,resultat_partie_joueur_1=?,rank_player_1_fin=?,rank_player_2_fin=?,elo_player_1_fin=?,elo_player_2_fin=? WHERE id=?",
                            [getDateTime(), plateau, point[0], point[1], statut_partie_joueur_1, rank_1, rank_2, new_elo_1, new_elo_2, id_partie],
                            function (err) {
                                if (err) callback('ERREUR');

                                if (statut_partie_joueur_1 == '1') {
                                    db.query(
                                    "UPDATE players SET game_play = game_play + 1,game_win=game_win + 1, elo = ?, rank=? WHERE id=?",
                                    [new_elo_1, rank_1, results[0].id_player_1],
                                    function (err) {
                                        if (err) callback('ERREUR');

                                        db.query(
                                        "UPDATE players SET game_play = game_play + 1,game_loose=game_loose + 1, elo = ?, rank=? WHERE id=?",
                                        [new_elo_2, rank_2, results[0].id_player_2],
                                        function (err) {
                                            if (err) callback('ERREUR');
                                            Determine_Side_Winner(results[0].id_player_1, results[0].id_player_2, id_partie, statut_partie_joueur_1, db, function () {
                                                callback();
                                            });

                                        });

                                    });
                                } else if (statut_partie_joueur_1 == '3') {
                                    db.query(
                                    "UPDATE players SET game_play = game_play + 1,game_win=game_win + 1, elo = ?, rank=? WHERE id=?",
                                    [new_elo_2, rank_2, results[0].id_player_2],
                                    function (err) {
                                        if (err) callback('ERREUR');

                                        db.query(
                                        "UPDATE players SET game_play = game_play + 1,game_loose=game_loose + 1, elo = ?, rank=? WHERE id=?",
                                        [new_elo_1, rank_1, results[0].id_player_1],
                                        function (err) {
                                            if (err) callback('ERREUR');
                                            Determine_Side_Winner(results[0].id_player_1, results[0].id_player_2, id_partie, statut_partie_joueur_1, db, function () {
                                                callback();
                                            });
                                        });
                                    });
                                } else {
                                    db.query(
                                    "UPDATE players SET game_play = game_play + 1,game_draw=game_draw + 1, elo = ?, rank=? WHERE id=?",
                                    [new_elo_2, rank_2, results[0].id_player_2],
                                    function (err) {
                                        if (err) callback('ERREUR');

                                        db.query(
                                        "UPDATE players SET game_play = game_play + 1,game_draw=game_draw + 1, elo = ?, rank=? WHERE id=?",
                                        [new_elo_1, rank_1, results[0].id_player_1],
                                        function (err) {
                                            if (err) callback('ERREUR');
                                            Determine_Side_Winner(results[0].id_player_1, results[0].id_player_2, id_partie, statut_partie_joueur_1, db, function () {
                                                callback();
                                            });
                                        });
                                    });
                                }
                            });
                        });
                    });
                });
            });
        });
}

var Determine_Side_Winner = function (id_player_1, id_player_2, id_partie, statut_partie_joueur_1, db, callback) {

    var id_player = 0;
    if (statut_partie_joueur_1 == 1) {
        id_player = parseInt(id_player_1);
    }
    else if (statut_partie_joueur_1 == 3) {
        id_player = parseInt(id_player_2);
    }

    if (id_player == 0) {
        db.query(
         "UPDATE game SET side_winner = 2 WHERE id=?",
         [id_partie],
         function (err) {
             if (err) callback('ERREUR');
             callback();
         });
    }
    else {
        db.query(
        "SELECT side_red FROM players WHERE id=?",
        [id_player],
        function (err, results) {
            if (err) callback('ERREUR');

            db.query(
                "UPDATE game SET side_winner = ? WHERE id=?",
                [results[0].side_red, id_partie],
                function (err) {
                    if (err) callback('ERREUR');
                    callback();
                });
        });
    }
}

var Client_Recuperation_Info_Fin_Partie = function (id_joueur, id_partie, db, callback) {
    /*var array = {
    nouveau_elo: '',
    nouveau_classement: '',
    resultat_partie: '',
    id_joueur_1,
    }*/
    //DETAIL INFO RETURN SEPARATEUR _
    var data_return = [];

    db.query(
    "SELECT id_player_1 FROM game WHERE id=?",
    [id_partie],
     function (err, result) {
         if (id_joueur == result[0].id_player_1) {
             db.query(
             "SELECT rank_player_1_fin, elo_player_1_fin,resultat_partie_joueur_1,point_player_1,point_player_2 FROM game WHERE id=?",
             [id_partie],
             function (err, results) {
                 data_return.push(results[0].elo_player_1_fin);
                 data_return.push(results[0].rank_player_1_fin);
                 data_return.push(results[0].resultat_partie_joueur_1);
                 data_return.push(result[0].id_player_1);
                 data_return.push(results[0].point_player_1);
                 data_return.push(results[0].point_player_2);
                 callback(data_return);
             });
         }
         else {
             db.query(
             "SELECT rank_player_2_fin, elo_player_2_fin,resultat_partie_joueur_1,point_player_1,point_player_2 FROM game WHERE id=?",
             [id_partie],
             function (err, results) {
                 data_return.push(results[0].elo_player_2_fin);
                 data_return.push(results[0].rank_player_2_fin);
                 data_return.push(results[0].resultat_partie_joueur_1);
                 data_return.push(result[0].id_player_1);
                 data_return.push(results[0].point_player_1);
                 data_return.push(results[0].point_player_2);
                 callback(data_return);
             });
         }
     });
}

var Remplissage_Plateau = function (plateau, statut_partie_case_a_remplir, callback) // 3 = Tour de joueur n1, 4 = Tour de joueur
{
    var new_plateau = plateau;
    for (var i in plateau) {
        if (plateau.charAt(i) == '0') {
            new_plateau = ReplaceAt(new_plateau, i, statut_partie_case_a_remplir.toString());
        }
    }
    callback(new_plateau);
}

var Calcule_Nouveau_Elo = function (EloActuel, EloAdversaire, statut_partie, callback) // statut_partie -> 1win 2 draw 3 loose
{
    var W = 0.0;
    var p_D = 0.0;
    var K = 0;
    var Elo_Actu = parseInt(EloActuel);
    var Elo_Opponent = parseInt(EloAdversaire);

    //INI VARIABLE
    if (statut_partie == '1') {
        W = 1;
    } else if (statut_partie == '2') {
        W = 0.5;
    } else {
        W = 0;
    }

    if (Elo_Actu > 2400 || Elo_Opponent > 2400) {
        K = 16;
    } else if (Elo_Actu < 2100 || Elo_Opponent < 2100) {
        K = 32;
    } else {
        K = 24;
    }

    var difference_elo = Elo_Actu - Elo_Opponent;
    p_D = 1 / (1 + Math.pow(10, (-difference_elo / 400)));

    var caca = Elo_Actu + (K * (W - p_D));
    callback(caca);
}

var Recuperation_Nombre_Point_Plateau = function (plateau) {
    var data = [];
    var joueur_1 = 0;
    var joueur_2 = 0;
    for (i = 0; i < plateau.length; i++) {
        if (plateau.charAt(i) == '3') {
            joueur_1++;
        }
        else if (plateau.charAt(i) == '4') {
            joueur_2++;
        }
    }
    data.push(joueur_1);
    data.push(joueur_2);
    return data;
}

var Get_Classement_Joueur = function (id_joueur) {
    var rank = 0;
    db.query(
    "SELECT id FROM players ORDER BY elo DESC",
     function (err, results) {
         for (var i in results) {
             if (results[i].id == id_joueur) {
                 rank = parseInt(i) + 1;
                 break;
             }
         }
         if (rank == 0) {
             rank = 999999;
         }
         return rank;
     });
}

var Get_Classement_Joueur_by_elo = function (elo, db, callback) {
    var rank = 0;
    var elo_int = parseInt(elo);
    db.query(
    "SELECT elo FROM players ORDER BY elo DESC",
     function (err, results) {
         if (err) callback('ERREUR');
         for (var i in results) {
             rank = parseInt(i) + 1;
             if (parseInt(results[i].elo) <= elo_int) {
                 break;
             }
         }
         if (rank == 0) {
             rank = 999999;
         }
         callback(rank);
     });
}

var Temps_Restant = function (date) {
    if (date == null || date == undefined) {
        return 20;
    }
    else {
        var blbl = date.split(":");
        var minutes = parseInt(blbl[0]);
        var secondes = parseInt(blbl[1]);

        var temps = new Date();
        var temps_min = temps.getMinutes();
        var temps_sec_now = temps.getSeconds();

        var total_secondes_next_time_before = minutes * 60 + secondes;
        var total_temps_sec_now = temps_min * 60 + temps_sec_now;

        if (total_temps_sec_now > total_secondes_next_time_before) {
            var blbl = 3600 - total_temps_sec_now + total_secondes_next_time_before;
            if (blbl < 30) {
                return blbl;
            }
            else {
                return 0;
            }
        }
        else {
            return total_secondes_next_time_before - total_temps_sec_now;
        }
    }
}

var getDateTime = function () {

    var date = new Date();

    var hour = date.getHours();
    hour = (hour < 10 ? "0" : "") + hour;

    var min  = date.getMinutes();
    min = (min < 10 ? "0" : "") + min;

    var sec  = date.getSeconds();
    sec = (sec < 10 ? "0" : "") + sec;

    var year = date.getFullYear();

    var month = date.getMonth() + 1;
    month = (month < 10 ? "0" : "") + month;

    var day  = date.getDate();
    day = (day < 10 ? "0" : "") + day;

    return year + ":" + month + ":" + day + " " + hour + ":" + min + ":" + sec;

}

var getDifferenceTemps_en_secondes = function (date) {
    var diff = {}                           // Initialisation du retour
    var date_actuel = new Date();
    var date_debut = new Date(date);

    var tmp = date_actuel - date_debut;

    tmp = Math.floor(tmp / 1000);             // Nombre de secondes entre les 2 dates
    diff.sec = tmp % 60;                    // Extraction du nombre de secondes

    tmp = Math.floor((tmp - diff.sec) / 60);    // Nombre de minutes (partie entiere)
    diff.min = tmp % 60;                    // Extraction du nombre de minutes

    tmp = Math.floor((tmp - diff.min) / 60);    // Nombre dheures (entieres)
    diff.hour = tmp % 24;                   // Extraction du nombre d'heures

    tmp = Math.floor((tmp - diff.hour) / 24);   // Nombre de jours restants
    diff.day = tmp;

    return (diff.day * 86400) + (diff.hour * 3600) + (diff.min * 60) + diff.sec;
}

var Verification_Case_Vide_Et_Possible = function (index, index_origin, plateau) {
    var NOMBRE_PLATEAU_LARGEUR = 8;
    var _index = parseInt(index);
    var _index_origin = parseInt(index_origin);

    if (_index >= 0 && _index < (NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_LARGEUR)) {
        if (plateau.charAt(_index) == '0') {
            if (Verifie_est_dans_les_cases_possible(_index_origin, _index)) {
                if (_index_origin % NOMBRE_PLATEAU_LARGEUR == 0 && (_index == _index_origin + NOMBRE_PLATEAU_LARGEUR - 1 || _index == _index_origin - NOMBRE_PLATEAU_LARGEUR - 1)) {
                    return false;
                }
                var modificateur_vertical = 0;
                if (_index == _index_origin - NOMBRE_PLATEAU_LARGEUR - 1 || _index == _index_origin - NOMBRE_PLATEAU_LARGEUR + 1 || _index == _index_origin - NOMBRE_PLATEAU_LARGEUR) {
                    modificateur_vertical = 1;
                } else if (_index == _index_origin + NOMBRE_PLATEAU_LARGEUR - 1 || _index == _index_origin + NOMBRE_PLATEAU_LARGEUR + 1 || _index == _index_origin + NOMBRE_PLATEAU_LARGEUR) {
                    modificateur_vertical = -1;
                } else if (_index == _index_origin - (NOMBRE_PLATEAU_LARGEUR * 2)) {
                    modificateur_vertical = 2;
                } else if (_index == _index_origin + (NOMBRE_PLATEAU_LARGEUR * 2)) {
                    modificateur_vertical = -2;
                }
                var index_base_horizontal = _index + (NOMBRE_PLATEAU_LARGEUR * modificateur_vertical);
                var base_horizontal = parseInt(index_base_horizontal / NOMBRE_PLATEAU_LARGEUR);
                var index_horizontal = parseInt(_index_origin / NOMBRE_PLATEAU_LARGEUR);

                if (base_horizontal == index_horizontal) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }
    return false;
}

var Verifie_est_dans_les_cases_possible = function (index_origine, index) {
    var NOMBRE_PLATEAU_LARGEUR = 8;
    var caca = [];
    var k = parseInt(index_origine);
    caca.push(k - 1);
    caca.push(k - 2);
    caca.push(k + 1);
    caca.push(k + 2);
    caca.push(k - NOMBRE_PLATEAU_LARGEUR);
    caca.push(k - NOMBRE_PLATEAU_LARGEUR - 1);
    caca.push(k - NOMBRE_PLATEAU_LARGEUR + 1);
    caca.push(k - (NOMBRE_PLATEAU_LARGEUR * 2));
    caca.push(k + NOMBRE_PLATEAU_LARGEUR);
    caca.push(k + NOMBRE_PLATEAU_LARGEUR + 1);
    caca.push(k + NOMBRE_PLATEAU_LARGEUR - 1);
    caca.push(k + (NOMBRE_PLATEAU_LARGEUR * 2));

    for (var i in caca) {
        if (caca[i] == index) {
            return true;
        }
    }
    return false;
}

var Verification_Possibilite_Jouer = function (id_plateau_joueur, plateau) {
    var NOMBRE_PLATEAU_LARGEUR = 8;
    var caca = [];
    for (k = 0; k < plateau.length; k++) {
        if (plateau.charAt(k) == id_plateau_joueur) {

            caca.push(k - 1);
            caca.push(k - 2);
            caca.push(k + 1);
            caca.push(k + 2);
            caca.push(k - NOMBRE_PLATEAU_LARGEUR);
            caca.push(k - NOMBRE_PLATEAU_LARGEUR - 1);
            caca.push(k - NOMBRE_PLATEAU_LARGEUR + 1);
            caca.push(k - (NOMBRE_PLATEAU_LARGEUR * 2));
            caca.push(k + NOMBRE_PLATEAU_LARGEUR);
            caca.push(k + NOMBRE_PLATEAU_LARGEUR + 1);
            caca.push(k + NOMBRE_PLATEAU_LARGEUR - 1);
            caca.push(k + (NOMBRE_PLATEAU_LARGEUR * 2));

            for (i = 0; i < caca.length; i++) {
                if (Verification_Case_Vide_Et_Possible(caca[i], k, plateau)) {
                    return true;
                }
            }
            caca = [];
        }
    }
    return false;
}

var Contamination = function (index_jouer, type_couleur, plateau, callback)// 3 ou 4 
{
    var NOMBRE_PLATEAU_LARGEUR = 8;
    var new_plateau = plateau;
    var array_index_plateau_change = [];
    var _index_jouer = parseInt(index_jouer);
    var type_case_index_jouer = parseInt(type_couleur);

    var type_case_autre;
    if (type_case_index_jouer == 3) {
        type_case_autre = 4;
    } else {
        type_case_autre = 3;
    }
    var caca = [];
    caca.push(_index_jouer - 1);
    caca.push(_index_jouer + 1);
    caca.push(_index_jouer - NOMBRE_PLATEAU_LARGEUR);
    caca.push(_index_jouer - NOMBRE_PLATEAU_LARGEUR - 1);
    caca.push(_index_jouer - NOMBRE_PLATEAU_LARGEUR + 1);
    caca.push(_index_jouer + NOMBRE_PLATEAU_LARGEUR);
    caca.push(_index_jouer + NOMBRE_PLATEAU_LARGEUR + 1);
    caca.push(_index_jouer + NOMBRE_PLATEAU_LARGEUR - 1);

    for (var i = 0; i < caca.length; i++) {
        if (Verification_Case_Possible(caca[i], _index_jouer) && parseInt(plateau.charAt(caca[i])) == type_case_autre) { 
            array_index_plateau_change[array_index_plateau_change.length] = caca[i];
            new_plateau = ReplaceAt(new_plateau, caca[i], type_case_index_jouer);
        }
    }

    if (array_index_plateau_change.length == 0) {
        array_index_plateau_change.push(-1);
    }

    var blbl = [];
    blbl.push(new_plateau);
    blbl.push(array_index_plateau_change);
    callback(blbl);
}

var ReplaceAt = function (texte, index, character) {
    var caca = [];
    for (var i = 0; i < texte.length; i++) {
        caca.push(texte[i]);
    }
    caca[index] = character;

    var blbl = "";
    for (var i in caca) {
        blbl = blbl + caca[i];
    }
    return blbl;
}


var Coup_Joueur_Plateau = function (index_jouer, plateau, type_couleur) {
    var new_plateau = plateau;
    new_plateau[index_jouer] = type_couleur;
    return new_plateau;
}

var Verification_Case_Possible = function (index, index_origin) {
    var NOMBRE_PLATEAU_LARGEUR = 8;
    var _index = parseInt(index);
    var _index_origin = parseInt(index_origin);

    if (_index >= 0 && _index < (NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_LARGEUR)) {

        if (_index_origin % NOMBRE_PLATEAU_LARGEUR == 0 && (_index == _index_origin + NOMBRE_PLATEAU_LARGEUR - 1 || _index == _index_origin - NOMBRE_PLATEAU_LARGEUR - 1)) {
            return false;
        }

        var modificateur_vertical = 0;
        if (_index == _index_origin - NOMBRE_PLATEAU_LARGEUR - 1 || _index == _index_origin - NOMBRE_PLATEAU_LARGEUR + 1 || _index == _index_origin - NOMBRE_PLATEAU_LARGEUR) {
            modificateur_vertical = 1;
        } else if (_index == _index_origin + NOMBRE_PLATEAU_LARGEUR - 1 || _index == _index_origin + NOMBRE_PLATEAU_LARGEUR + 1 || _index == _index_origin + NOMBRE_PLATEAU_LARGEUR) {
            modificateur_vertical = -1;
        } else if (_index == _index_origin - (NOMBRE_PLATEAU_LARGEUR * 2)) {
            modificateur_vertical = 2;
        } else if (_index == _index_origin + (NOMBRE_PLATEAU_LARGEUR * 2)) {
            modificateur_vertical = -2;
        }
        var index_base_horizontal = _index + (NOMBRE_PLATEAU_LARGEUR * modificateur_vertical);
        var base_horizontal = parseInt(index_base_horizontal / NOMBRE_PLATEAU_LARGEUR);
        var index_horizontal = parseInt(_index_origin / NOMBRE_PLATEAU_LARGEUR);
        if (base_horizontal == index_horizontal) {
            return true;
        }
    }
    return false;
}

var RandomQuiJoueEnPremier = function()
{
    var rand = Math.random();
    if(rand >0.5)
    {
        return true;
    }
    else
    {
        return false;
    }
}

var Plateau_Initialization = function () {
    var index = RANDOM(0, 9);
    switch (index) {
        case 0:
            return "3000000000000000000000000000000000000000000000000000000000000004";
        case 1:
            return "3000000000000000000000000001100000011000000000000000000000000004";
        case 2:
            return "3300000031111100010000100101001001001010010000100011111400000044";
        case 3:
            return "3101100001100100010000100100001001000010010000100010011000011014";
        case 4:
            return "0000000110000010010101000013100000141000010101001000001000000001";
        case 5:
            return "1110011111000011100000013000000430000004100000011100001111100111";
        case 6:
            return "3100100010100100010100100010100110010100010010100010010100010014";
        case 7:
            return "3001400000010000000100030041111111111400300010000000100000041003";
        case 8:
            return "1400000141000010001001000001100000011000001001000100001310000031";
        default:
            return "3000000000000000000000000000000000000000000000000000000000000004";
    }
}

/* TYPE HISTORIQUE PARTIE
separateur entre coup -> ! 
separateur entre info -> _
separateur entre conta_info -> ?

placement info dans coup
1 - id_joueur
2 - type de coup (0 = evol, 1= saut, 2= pas_assez_temps)
3 - index_origin
4 - index_destination
5 - DATETIME du coup
6 - Index Contamination Affecte

*/

var AjoutCoupString = function (historique, id_joueur, type_de_coup, index_origin, index_destination, index_contamination) {
    var time = Get_minute_seconde_from_date();
    var coup = id_joueur.toString() + "_" + type_de_coup.toString() + "_" + index_origin.toString() + "_" + index_destination.toString() + "_" + time + "_" + index_contamination;
    if (historique == null || historique == undefined) {
        return coup;
    }
    else {
        return historique + "!" + coup;
    }
}

var Client_Recuperation_Data_Dernier_Coup = function (historique) {
    var coup_array = historique.split("!");
    coup_array.reverse();
    var data = coup_array[0];
    return data;
}

var Verification_Adversaire_Jouer = function (id_joueur, id_partie, historique, db) {
    if (historique == null || historique == undefined) {
        return false;
    }
    else {
        var data = Client_Recuperation_Data_Dernier_Coup(historique);
        var blbl = data.split('_');
        if (blbl[0] == id_joueur) {
            return false;
        }
        else {
            return true;
        }
    }
}

exports.Update_Info_General_Parties = function (db) {
    var date_now = getDateTime();
    db.query(
    "INSERT INTO info_general (date)" +
    "VALUES(?) ",
   [date_now],
   function (err, results) {

       var id = results.insertId;

       db.query(
        "SELECT COUNT(*) AS red_win FROM game WHERE side_winner = 1",
        function (err, blbl_1) {
            db.query(
            "SELECT COUNT(*) AS green_win FROM game WHERE side_winner = 0",
            function (err, blbl_2) {
                db.query(
                "SELECT COUNT(*) AS nbr_game FROM game",
                function (err, blbl_3) {
                    db.query(
                    "SELECT COUNT(*) AS nbr_partie_en_cours FROM game WHERE statut_partie = 3 OR statut_partie = 4",
                    function (err, blbl_4) {
                        db.query(
                        "SELECT COUNT(*) AS nbr_players FROM players",
                        function (err, blbl_5) {
                            db.query(
                            "SELECT COUNT(*) AS nbr_de_connard_vert FROM players WHERE side_red = 0",
                            function (err, blbl_6) {
                                var nbr_connard_red = parseInt(blbl_5[0].nbr_players) - parseInt(blbl_6[0].nbr_de_connard_vert);
                                db.query(
                                "UPDATE info_general SET nombre_game_total =  ? , nombre_win_green_total =  ? , nombre_win_red_total = ?,nbr_joueur =?, nbr_partie_en_cours=?, nbr_green_player=?, nbr_red_player=?  WHERE id= ? ",
                                [blbl_3[0].nbr_game, blbl_2[0].green_win, blbl_1[0].red_win, blbl_5[0].nbr_players, blbl_4[0].nbr_partie_en_cours, blbl_6[0].nbr_de_connard_vert, nbr_connard_red.toString(), id],
                                function (err) {
                                });
                            });
                        });
                    });
                });
            });
        });
   });
};

//AUTHENTIFICATION UTILISATEUR
var Generation_code_client = function () {
    var data = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "M", "Z", "X", "C", "V", "B", "N", "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "m", "z", "x", "c", "v", "b", "n"];
    var caca = "";
    for (i = 0; i < 20; i++) {
        var index = RANDOM(0, data.length);
        caca = caca + data[index];
    }
    return caca;
}

var RANDOM = function (min, max) { // min inclu, max exclu
  return Math.floor( Math.random() * (max - min)) + min;
}

exports.Verifier_Client = function (db, id_client, code, callback) {
    if (id_client != undefined && code != undefined) {
        db.query(
    "SELECT code FROM players WHERE id=?",
    [id_client],
     function (err, results) {
         if (err) callback('ERREUR');
         if (results[0].code == code) {
             callback(true);
         }
         else {
             callback(false);
         }
     });
    }
    else {
        callback('ERREUR');
    }
};
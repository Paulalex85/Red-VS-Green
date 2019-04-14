var net = require('net');
var rvg = require('./rvg');
var mysql = require('mysql');

var HOST = '';
var PORT = ;

var Wait_Room = [];
var client = [];

var db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: '',
    database: 'rvg'
});

var server = net.createServer(function (socket) {
    //add client
    console.log('CONNECTED: ' + socket.remoteAddress + ':' + socket.remotePort);
    socket.name = socket.remoteAddress + ':' + socket.remotePort;
    client.push(socket);


    socket.on('data', function (data) {
        console.log('DATA ' + socket.remoteAddress + ': ' + data);

        //split info
        var data_string = data.toString();
        var array_data = data_string.split(' ');
        //RECUPERE ID ACTION
        var id = parseInt(array_data[0]);

        switch (id) {
            //Creation Nouveau Joueur                                                                                     
            case 1:
                var name = array_data[1]; // name 15 caractere
                var color = parseInt(array_data[2]); // color 0 = green 1 = red
                rvg.Nouveau_Joueur(name, color, db, function (info) {
                    console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                    socket.write(info);
                });
                break;

            //Verification Nom pris                                                                              
            case 2:
                var name = array_data[1];
                rvg.Verification_Name_Already_Taken(name, db, function (info) {
                    console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                    socket.write(info);
                });
                break;

            //Recuperation Info Menu                                                                               
            case 3:
                var id_joueur = array_data[1];
                var name = array_data[2];
                rvg.Recuperation_Data_Main_Menu(id_joueur,name, db, function (info) {
                    console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                    socket.write(info);
                });
                break;

            //Client Validation Partie Trouver ( Accept or Refuse )                                                                       
            case 4:
                var id_joueur = array_data[1];
                var id_partie = array_data[2];
                var choix = array_data[4]; // 0 = refuse | 1 = accept 
                var code = array_data[3];
                rvg.Verifier_Client(db, id_joueur, code, function (is_auth) {
                    if (is_auth) {
                        rvg.Client_Verification_Validation_Partie(id_joueur, id_partie, choix, db, function (info) {
                            console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                            socket.write(info);
                        });
                    }
                    else {
                        var info = "ERREUR_AUTH";
                        console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                        socket.write(info);
                    }
                });


                break;

            //Client Passer Game en pret                                                                        
            case 5:
                var id_partie = array_data[2];
                var id_joueur = array_data[1];
                var code = array_data[3];
                rvg.Verifier_Client(db, id_joueur, code, function (is_auth) {
                    if (is_auth) {
                        rvg.Client_Passe_Partie_En_Pret(id_partie, id_joueur, db, function (info) {
                            var data = '5 ' + info;
                            console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + data);
                            socket.write(data);
                        });
                    }
                    else {
                        var info = "ERREUR_AUTH";
                        console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                        socket.write(info);
                    }
                });
                break;

            //Client ENregistrer coup Joueur                                                                    
            case 6:
                var id_partie = array_data[2];
                var id_joueur = array_data[1];
                var detail_coup = array_data[3]; // 0 = type_coup (0 = evol, 1= saut, 2= pas_assez_temps) , 1= index_origin, 2= index   SEPARATEUR -> _
                var code = array_data[4];

                rvg.Verifier_Client(db, id_joueur, code, function (is_auth) {
                    if (is_auth) {
                        rvg.Client_Coup_Jouer_Joueur(id_partie, id_joueur, detail_coup, db, function (info) {
                            var data = '6 ' + info;
                            console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + data);
                            socket.write(data);
                        });
                    }
                    else {
                        var info = "ERREUR";
                        console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                        socket.write(info);
                    }
                });
                break;

            //Verifier si adversaire a jouer                                                               
            case 7:
                var id_partie = array_data[2];
                var id_joueur = array_data[1];
                var code = array_data[3];
                rvg.Verifier_Client(db, id_joueur, code, function (is_auth) {
                    if (is_auth) {
                        rvg.Client_Verification_Si_Adversaire_A_Jouer(id_partie, id_joueur, db, function (info) {
                            var data = '7 ' + info;
                            console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + data);
                            socket.write(data);
                        });
                    }
                    else {
                        var info = "ERREUR";
                        console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                        socket.write(info);
                    }
                });

                break;

            // Ajout Joueur dans liste attente partie                                                             
            case 8:
                var id_joueur = array_data[1];
                var code = array_data[2];
                rvg.Verifier_Client(db, id_joueur, code, function (is_auth) {
                    if (is_auth) {
                        Wait_Room.push(id_joueur);
                        socket.id = id_joueur;
                        console.log('Add ' + id_joueur.toString() + ' in wait room');
                        console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + 'true');
                        socket.write('true');
                    }
                    else {
                        console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + 'false');
                        socket.write('false');
                    }
                });
                break;

            //Sortir file attente            
            case 9:
                var id_joueur = array_data[1];
                Quitter_file_attente(socket);
                break;

            //Obtenir info game en cours     
            case 10:
                var id_partie = array_data[1];
                var id_joueur = array_data[2];
                rvg.Client_Recuperer_Info_Game_En_Cours(id_partie, id_joueur, db, function (info) {
                    var data = '10 ' + info;
                    console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + data);
                    socket.write(data);
                });
                break;

            //Verification Partie Trouve    
            case 11:
                var id_joueur = array_data[1];
                var code = array_data[2];
                rvg.Verifier_Client(db, id_joueur, code, function (is_auth) {
                    if (is_auth) {
                        rvg.Client_Verifier_Partie_Trouver(id_joueur, db, function (info) {
                            console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                            socket.write(info);
                        });
                    }
                    else {
                        var info = "ERREUR";
                        console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                        socket.write(info);
                    }
                });
                break;

            //Obtenir temps restant partie     
            case 12:
                var id_partie = array_data[1];
                var id_joueur = array_data[2];
                rvg.Client_Recuperer_Temps_Partie(id_partie, id_joueur, db, function (info) {
                    var data = '12 ' + info;
                    console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + data);
                    socket.write(data);

                    /*.Get_Next_time_before_this_time(db, id_partie, function (blbl) {
                        console.log('NEXT TIME BEFORE THIS TIME: ' + blbl);
                    });*/
                });
                break;

            //Abandonner partie  
            case 13:
                var id_joueur = array_data[1];
                var code = array_data[3];
                var id_partie = array_data[2];
                rvg.Verifier_Client(db, id_joueur, code, function (is_auth) {
                    if (is_auth) {
                        rvg.Client_Abandonner_Partie(id_partie, id_joueur, db, function (info) {
                            console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                            socket.write(info);
                        });
                    }
                    else {
                        var info = "ERREUR";
                        console.log('DATA SEND TO ' + socket.remoteAddress + ': ' + info);
                        socket.write(info);
                    }
                });
                break;

            default:
                socket.write('');
                break;
        }
    });

    socket.on('close', function (data) {
        console.log('CLOSED: ' + socket.name);
        Quitter_file_attente(socket);
        client.splice(client.indexOf(socket), 1);

    });
}).listen(PORT, HOST);

console.log('Server listening on ' + HOST +':'+ PORT);

setInterval( function() { rvg.Creation_Partie(Wait_Room, db)}, 1000);
setInterval( function() { rvg.Delete_Parties_en_Attente(db)}, 5000);
setInterval( function() { rvg.Verification_Temps_Depassement(db)}, 3000);
setInterval( function() { rvg.Update_Info_General_Parties(db)}, 60000);
setInterval( function() { rvg.Defaite_Apres_Absence_Prolonge(db)}, 4000);

var INFINITE_LOOP = function () {
    var x = 1;
    while (x > 0) {
        x++;
        console.log(x.toString());
    }
}

var Quitter_file_attente = function (socket) {
    if (Wait_Room.indexOf(socket.id) != -1) {
        Wait_Room.splice(Wait_Room.indexOf(socket.id), 1);
        console.log('Leave the wait room: ' + socket.id);
    }
}

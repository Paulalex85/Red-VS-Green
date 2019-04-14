using System; 
using System.Threading; 
using System.Net; 
using System.Net.Sockets;
using System.Text;

namespace RedVsGreen
{
	public class tcp_server
	{
		int i;
		TcpClient client = new TcpClient (); // Creates a TCP Client 
		NetworkStream stream; //Creats a NetworkStream (used for sending and receiving data) 
		byte[] datalength = new byte[4]; // creates a new byte with length 4 ( used for receivng data's lenght) 
		bool _connected = false;
		bool _reception_message_en_cours = false;
		IPAddress ip = IPAddress.Parse("37.187.107.94");
		int port = 1337;
		private Thread message_thread;
		public string message = "";


		public tcp_server ()
		{
		}

		public string Connect()
		{
			try {
				client.Connect (ip, port);
			} catch (Exception ex) {
				return ex.ToString();
			}
			return "";
		}

		public void clientSend(string msg)
		{
			try
			{
				stream = client.GetStream(); //Gets The Stream of The Connection
				byte[] data; // creates a new byte without mentioning the size of it cuz its a byte used for sending
				data = Encoding.Default.GetBytes(msg); // put the msg in the byte ( it automaticly uses the size of the msg )
				int length = data.Length; // Gets the length of the byte data
				byte[] datalength = new byte[4]; // Creates a new byte with length of 4
				datalength = BitConverter.GetBytes(length); //put the length in a byte to send it
				stream.Write(data, 0, data.Length); //Sends the real data

				clientReceive();
			}
			catch (Exception ex)
			{
			}
		} 

		private void clientReceive()
		{
			try
			{
				stream = client.GetStream(); //Gets The Stream of The Connection
				message_thread = new Thread(Reception_message_thread);
				message_thread.Start();
			}
			catch (Exception ex)
			{
			}
		}

		private void Reception_message_thread()
		{
			byte[] data_receive = new byte[1024];
			byte[] data;
			stream.Read (data_receive, 0, data_receive.Length);

			int i = 0;
			while (data_receive [i] != 0) {
				i++;
			}
			data = new byte[i];
			Array.Copy (data_receive, data, i);
			message = Encoding.ASCII.GetString(data);
		}
	}
}


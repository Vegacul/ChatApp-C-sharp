using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Communication
{
    public class Message
    {


        public static void sendMsg(Stream s, List<String> msg)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(s, msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public static List<String> rcvMsg(Stream s)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (List<String>)bf.Deserialize(s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new List<String>();

        }
    }
}
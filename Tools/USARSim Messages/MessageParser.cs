using System;
using System.Text;
using System.Collections.Generic;

namespace ScanMatchers.Tools.USARSimMessages
{

    public enum eSimulationMessageType
    {
        None = -1,
        INS = 10,
        Laser,
        Odometdy,
        GroundTruth
    };

    public class MessageParser
    {

        private string SimMsg = "";
        private int SimMsgType = 0;
        private object SimMsgData = null;

        private void ParseMessage(string message)
        {
			USARParser msg = new USARParser(message);
            try
            {
                if (msg.type.Equals("SEN"))
                {
                    string SensorType = msg.getSegment("Type").Get("Type");
                    if (SensorType.Equals("GroundTruth"))
                    {
                        SimMsgType = (int)eSimulationMessageType.GroundTruth;
                        SimMsgData = new GroundTruth(msg);
                    }
                    else if (SensorType.Equals("Odometry"))
                    {
                        SimMsgType = (int)eSimulationMessageType.Odometdy;
                        SimMsgData = new Odometry(msg);
                    }
                    else if (SensorType.Equals("INS"))
                    {
                        SimMsgType = (int)eSimulationMessageType.INS;
                        SimMsgData = new INS(msg);
                    }
                    else if (SensorType.Equals("RangeScanner"))
                    {
                        SimMsgType = (int)eSimulationMessageType.Laser;
                        SimMsgData = new Laser(msg);
                    }
                    else
                    {
                        SimMsgData = null;
                        SimMsgType = -1;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Unable parse message <" + message + "> into object: ");
            }
        }

        public MessageParser()
        {
            SimMsg = "";
            SimMsgType = -1;
        }

        public string SimulationMessage
        {
            get { return SimMsg; }
            set {
                    if (!string.IsNullOrEmpty(value.Trim()))
                    {
                        SimMsg = value;
                        ParseMessage(SimMsg);
                    }
                    else
                    {
                        SimMsg = "";
                    }
                }
        }

        public int MessageType
        {
            get { return SimMsgType; }
        }

        public object MessageData
        {
            get { return SimMsgData; }
        }

    }

}

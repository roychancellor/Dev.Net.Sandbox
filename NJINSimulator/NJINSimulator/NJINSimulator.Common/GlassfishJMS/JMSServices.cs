using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Transport.Discovery;
using NJINSimulator.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Common.GlassfishJMS
{
    public sealed class JMSServices
    {
        private static readonly JMSServices _instance = new JMSServices();
        private static IConnection _connection;
        private static ISession _session;

        static JMSServices()
        {
        }
        private JMSServices()
        {
            Init();
        }
        public static JMSServices Instance
        {
            get
            {
                return _instance;
            }
        }

        public IConnection Connection { get { return _connection; } }
        public ISession Session { get { return _session; } }

        public static void Init()
        {
            var connecturi = new Uri("tcp://localhost:8080"); // TODO: Put in config file
            var connectionFactory = new ConnectionFactory(connecturi);

            /***************************************************/
            //DiscoveryAgentFactory discoveryAgentFactory = new DiscoveryAgentFactory();
            //discoveryAgentFactory.RegisterAgentFactory("jms/__defaultConnectionFactory", typeof(DiscoveryAgentFactory));
            //InitialContext initialContext = null;
            //initialContext = new InitialContext();
            ////Step-1 Create ConnectionFactory
            //ConnectionFactory connectionFactory
            //    = (ConnectionFactory)initialContext.lookup("jms/__defaultConnectionFactory");
            ////Step-2 Create connection
            //Connection connection = connectionFactory.createConnection();
            /***************************************************/

            try
            {
                // Create a Connection
                _connection = connectionFactory.CreateConnection();
                _connection.Start();

                // Create a Session
                _session = _connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IDestination Destination(string toDestination, bool isTopic = false)
        {
            if (_session == null || toDestination.IsNullOrEmpty())
            {
                return null;
            }

            IDestination destination;
            if (isTopic)
            {
                destination = _session.GetTopic(toDestination);
            }
            else
            {
                destination = _session.GetQueue(toDestination);
            }
            if (destination == null)
            {
                return null;
            }
            return destination;
        }
        
        public IMessageProducer Producer(string toDestination, bool isTopic = false)
        {
            IMessageProducer producer;

            if (_session == null || toDestination.IsNullOrEmpty())
            {
                return null;
            }
            try
            {
                // Get the destination (Topic or Queue)
                var destination = Destination(toDestination, isTopic);
                if (destination == null)
                {
                    return null;
                }

                // Create a MessageProducer from the Session to the Topic or Queue
                producer = _session.CreateProducer(destination);
                if (producer == null)
                {
                    return null;
                }

                if (!isTopic) producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            }
            catch (Exception)
            {
                return null;
            }

            return producer;
        }

        public IMessageConsumer Consumer(string toQueue)
        {
            IMessageConsumer consumer;

            if (_session == null || toQueue.IsNullOrEmpty())
            {
                return null;
            }
            try
            {
                // Get the destination (Topic or Queue)
                var destination = Destination(toQueue);
                if (destination == null)
                {
                    return null;
                }

                // Create a MessageProducer from the Session to the Topic or Queue
                consumer = _session.CreateConsumer(destination);
                if (consumer == null)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return consumer;
        }

        public bool Send(IMessageProducer producer, string theMessage)
        {
            if (_session == null)
            {
                throw new Exception($"Send: _session is NULL");
                //return false;
            }
            if (producer == null)
            {
                throw new Exception($"Send: producer is NULL");
                //return false;
            }
            if (theMessage.IsNullOrEmpty())
            {
                throw new Exception($"Send: theMessage is NULL or EMPTY");
                //return false;
            }

            try
            {
                // Create a messages
                ITextMessage message = _session.CreateTextMessage(theMessage);

                // Tell the producer to send the message
                producer.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }

            return true;
        }

        public bool Close()
        {
            if (_session == null || _connection == null)
            {
                return false;
            }
            _session.Close();
            _connection.Close();
            return true;
        }
    }
}

NJIN Simulator HOW-TO

This application simulates NJIN 2.0 and the PMB NSB functions, along with a PROBE/UFO simulator for TPS and Delta T through the NJIN. The NJIN modules communicate via ActiveMQ queues and have no functionality other than to sleep for a configurable amount of time.

There are three Console applications and two WebApi applications that are hosted in IIS:
1. NJINSimulator.Application: Configures all NJIN modules to listen to their associated queues which are configured in App.config.
2. PROBESimulator.JMS.Application: Configures PROBE to listen to associated topics which are hard-coded in PROBEServicesSingleton (TODO: Put in App.config).
3. IOSimulator.NSB.OUT: Reads test configuration data from a file, builds TransactionalWrapper objects (very thin mocks), and sends POST requests to the IO Interface. Location of test config file and URL to IO interface are in App.config.
4. NJINSimulator.IO: Contains two controllers with POST methods, one for the IO Interface (from NSB.OUT) and one for NSB.IN (from Sender Manager). Both controllers provide information via Swagger.
5. UFOSimulator.IO: Provides controllers for TPS and Delta T.

Operation:
1. Start the NJINSimulator.Application: C:\dev.net\Sandbox\NJINSimulator\NJINSimulator\NJINSimulator.Application\bin\Debug\NJINSimulator.Application.exe
2. Start the PROBESimulator.JSM.Application: C:\dev.net\Sandbox\NJINSimulator\NJINSimulator\PROBESimulator.JMS.Application\bin\Debug\PROBESimulator.Application.exe
3. Open logs in C:\dev.net\Sandbox\_Logs
4. Open ActiveMQ Admin console: http://localhost:8161/admin/queues.jsp (UN: admin, PW: admin). To reset the counts, restart the Windows service.
5. Open the Swagger page for the NJIN simulator: http://localhost:9199/swagger/ui/index#/
6. Open the Swagger page for the PROBE/UFO simulator: http://localhost:9200/swagger/ui/index#/
7. Open Postman and navigate to the NJIN Simulator collection. Send a POST to http://localhost:9199/njinsimulator/io/api/v1/IOInterface with the below body and verify by logs and by the ActiveMQ console that the message went through the NJIN.
8. To test the NSB.IN functionality, send a POST to http://localhost:9199/njinsimulator/io/api/v1/NSBIn using the completed TW below.
9. Verify the test configuration: C:\dev.net\Sandbox\NJINSimulator\NJINSimulator\IOSimulator.NSB.OUT\TestConfig\TestConfig.xml
10. To run a test, start the IOSimulator.NSB.OUT application: C:\dev.net\Sandbox\NJINSimulator\NJINSimulator\IOSimulator.NSB.OUT\bin\Debug\IOSimulator.NSB.OUT.exe.
11. Monitor the tests via logs and the ActiveMQ console.

Initial Transactional Wrapper (to IO Interface):
<TransactionalWrapper>
    <Verbose>true</Verbose>
    <InboundCorrelationID>1234567890XML</InboundCorrelationID>
</TransactionalWrapper>

Completed Transactional Wrapper (to NSB.IN)
<TransactionalWrapper>
    <Verbose>true</Verbose>
    <InboundCorrelationID>1234567890XML</InboundCorrelationID>
    <ProcessResults>
		<ProcessResult>01:IO Interface</ProcessResult>
		<ProcessResult>02:Request Manager</ProcessResult>
		<ProcessResult>03:PO</ProcessResult>
		<ProcessResult>04:Router Dest</ProcessResult>
		<ProcessResult>05:Router Route</ProcessResult>
		<ProcessResult>06:Image Processor</ProcessResult>
		<ProcessResult>07:Conversion</ProcessResult>
		<ProcessResult>08:Sender Manager</ProcessResult>
	</ProcessResults>
</TransactionalWrapper>

Logic:
Each NJIN module is an AbstractNJINService which has three methods: Init, Handle, and ToString which the concrete modules can override. The abstract class also has several properties to define the source, destination, a Producer, a Consumer, and process time. The default implementation of the Handle method is for queue-to-queue modules which simply consumes from a queue and publishes to a queue with a delay in between.
A few modules have their own concrete implementation and override the Handle method with unique logic to the module, such as Sender Manager and PO out.
The NJINServicesSingleton class initializes all NJIN modules and stores them in a Dictionary.
The IO Interface is in its own singleton: NJINServicesIOSingleton
The PROBE services work similarly to the NJIN modules. Tehre is an AbstracePROBEService class that defines Init and Handle methods. The PROBE modules consume from a topic, perform an operation, and send the result to the UFO via HTTP.
The UFO controllers (TPS or DeltaT) receive the POST request and process it accordingly, which right now is to compute the moving average and log it.
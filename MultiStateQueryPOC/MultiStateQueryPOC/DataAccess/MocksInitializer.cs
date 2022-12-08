using MultiStateQueryPOC.Models.Common;
using MultiStateQueryPOC.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultiStateQueryPOC.DataAccess
{
    public class MocksInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<QueryContext>
    {
        protected override void Seed(QueryContext context)
        {
            var mocks = new List<Mock>
            {
                new Mock { MessageKey = "DR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.CANDLE30.ToString(), MockBody = "MOCK BODY DR - AZ", },
                new Mock { MessageKey = "RR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.CANDLE30.ToString(), MockBody = "MOCK BODY RR - AZ", },
                new Mock { MessageKey = "CR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.RAP41.ToString(), MockBody = "MOCK BODY CR - AZ", },
                new Mock { MessageKey = "FR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.RAP41.ToString(), MockBody = "MOCK BODY FR - AZ", },
                new Mock { MessageKey = "DR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.CANDLE30.ToString(), MockBody = "MOCK BODY DR - CO", },
                new Mock { MessageKey = "RR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.CANDLE30.ToString(), MockBody = "MOCK BODY RR - CO", },
                new Mock { MessageKey = "CR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.RAP41.ToString(), MockBody = "MOCK BODY CR - CO", },
                new Mock { MessageKey = "FR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.RAP41.ToString(), MockBody = "MOCK BODY FR - CO", },
                new Mock { MessageKey = "DR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.CANDLE30.ToString(), MockBody = "MOCK BODY DR - NM", },
                new Mock { MessageKey = "RR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.CANDLE30.ToString(), MockBody = "MOCK BODY RR - NM", },
                new Mock { MessageKey = "CR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.RAP41.ToString(), MockBody = "MOCK BODY CR - NM", },
                new Mock { MessageKey = "FR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.RAP41.ToString(), MockBody = "MOCK BODY FR - NM", },
                new Mock { MessageKey = "DR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.CANDLE30.ToString(), MockBody = "MOCK BODY DR - UT", },
                new Mock { MessageKey = "RR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.CANDLE30.ToString(), MockBody = "MOCK BODY RR - UT", },
                new Mock { MessageKey = "CR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.RAP41.ToString(), MockBody = "MOCK BODY CR - UT", },
                new Mock { MessageKey = "FR", HeaderType = Format.NLETS2.ToString(), PayloadType = Format.RAP41.ToString(), MockBody = "MOCK BODY FR - UT", },
            };

            mocks.ForEach(m => context.Mocks.Add(m));
            context.SaveChanges();

            var states = new List<State>
            {
                new State { StateCode = "AZ", MockID = 1, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "AZOLN0000" },
                new State { StateCode = "AZ", MockID = 2, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "AZOLN0000" },
                new State { StateCode = "AZ", MockID = 3, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "AZOLN0000" },
                new State { StateCode = "AZ", MockID = 4, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "AZOLN0000" },
                new State { StateCode = "CO", MockID = 5, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "COOLN0000" },
                new State { StateCode = "CO", MockID = 6, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "COOLN0000" },
                new State { StateCode = "CO", MockID = 7, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "COOLN0000" },
                new State { StateCode = "CO", MockID = 8, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "COOLN0000" },
                new State { StateCode = "NM", MockID = 9, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "NMOLN0000" },
                new State { StateCode = "NM", MockID = 10, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "NMOLN0000" },
                new State { StateCode = "NM", MockID = 11, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "NMOLN0000" },
                new State { StateCode = "NM", MockID = 12, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "NMOLN0000" },
                new State { StateCode = "UT", MockID = 13, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "UTOLN0000" },
                new State { StateCode = "UT", MockID = 14, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "UTOLN0000" },
                new State { StateCode = "UT", MockID = 15, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "UTOLN0000" },
                new State { StateCode = "UT", MockID = 16, SendHeaderType = Format.NLETS2.ToString(), SendPayloadType = Format.NLETS2.ToString(), SendPayloadCHRIType = Format.RAP41.ToString(), ResponseDelayMilliseconds = 100, SendORI = "UTOLN0000" },
            };

            states.ForEach(st => context.States.Add(st));
            context.SaveChanges();
        }
    }
}
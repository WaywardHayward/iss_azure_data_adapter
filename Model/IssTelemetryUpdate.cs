using com.lightstreamer.client;

namespace iss_data.Model
{
    public class IssTelemetryUpdate : Symbol
    {
        public string Discipline { get; set; }
        public string MessageType {get;set;} = "IssSensor";        
        public string Status { get; private set; }
        public string Indicator { get; private set; }
        public string Color { get; private set; }
        public string TimeStamp { get; private set; }
        public string Value { get; private set; }
        public string CalibratedData { get; private set; }

        public static IssTelemetryUpdate FromSymbol(Symbol symbol, ItemUpdate update)
        {
            var telemetryUpdate = new IssTelemetryUpdate
            {
                PUI = symbol.PUI,
                PublicPUI = symbol.PublicPUI,
                Description = symbol.Description,
                MIN = symbol.MIN,
                MAX = symbol.MAX,
                OPSNOM = symbol.OPSNOM,
                ENGNOM = symbol.ENGNOM,
                UNITS = symbol.UNITS,
                ENUM = symbol.ENUM,
                FormatSpec = symbol.FormatSpec
            };

            telemetryUpdate.Status = update.getValue("Status.Class");
            telemetryUpdate.Indicator = update.getValue("Status.Indicator");
            telemetryUpdate.Color = update.getValue("Status.Color");
            telemetryUpdate.TimeStamp = update.getValue("TimeStamp");
            telemetryUpdate.Value = update.getValue("Value");
            telemetryUpdate.CalibratedData = update.getValue("CalibratedData");

            return telemetryUpdate;
        }

    }

}
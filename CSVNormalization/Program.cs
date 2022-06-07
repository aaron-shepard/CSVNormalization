
using CsvHelper;
using CsvHelper.Configuration;
using CSVNormalization;
using System.Globalization;
using System.Text;

var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Encoding = Encoding.UTF8,
    Delimiter = ","
};

using (StreamReader textReader = new(Console.OpenStandardInput(), Encoding.UTF8))
using (CsvReader csv = new(textReader, configuration))
{
    var clientRecordList = csv.GetRecords<ClientRecord>();

    using (TextWriter writer = new StreamWriter(Console.OpenStandardOutput()))
    {
        CsvWriter csvWriter = new(writer, configuration);

        csvWriter.WriteHeader<ClientRecord>();
        csvWriter.NextRecord();
        foreach (var clientRecord in clientRecordList)
        {
            clientRecord.NormalizeData();
            csvWriter.WriteRecord<ClientRecord>(clientRecord);
            csvWriter.NextRecord();
        }
    }
}

using CsvHelper;
using CsvHelper.Configuration;
using CSVNormalization;
using System.Globalization;
using System.Text;

var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Encoding = Encoding.UTF8,
    Delimiter = ","
};

// TODO - validate CSV file is inputted
// add error/warning to console to indicate bad CSV
using (StreamReader textReader = new(Console.OpenStandardInput(), Encoding.UTF8))
using (CsvReader csv = new(textReader, csvConfiguration))
{
    var clientRecordList = csv.GetRecords<ClientRecord>();

    using TextWriter writer = new StreamWriter(Console.OpenStandardOutput());
    CsvWriter csvWriter = new(writer, csvConfiguration);

    csvWriter.WriteHeader<ClientRecord>();
    csvWriter.NextRecord();
    foreach (var clientRecord in clientRecordList)
    {
        if (clientRecord.NormalizeData())
        {
            csvWriter.WriteRecord<ClientRecord>(clientRecord);
            csvWriter.NextRecord();
        }
    }
}
// See https://aka.ms/new-console-template for more information

using Alh.AllocatePayroll;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;


var workingDirectory = Directory.GetCurrentDirectory().Replace(@"bin\Debug\net6.0", "");
const string payrollAllocation = "PayrollAllocTable.csv";
const string payrollEntries = "payrollentries.csv";

var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Delimiter = ","
};

Dictionary<string, PayrollAllocation> payrollAllocationDictionary = GetPayrollAllocation(workingDirectory, payrollAllocation, config);

var entries = new List<PayrollEntry>();

using (var reader = new StreamReader($"{workingDirectory}\\{payrollEntries}"))
using (var csv = new CsvReader(reader, config))
{
    var records = csv.GetRecords<PayrollDetails>();
    foreach (var record in records)
    {
     
        var key = $"{record.Name}-{record.PayDate.Month}";

        var allocation = payrollAllocationDictionary[key];

        decimal amountTobeAllocated = decimal.Parse(record.Amount);

        // ????? 
        decimal bocaRatonAllocatedAmount = Math.Round(amountTobeAllocated * allocation.BocaRatonAllocation, 2);
        decimal coralSpringsAllocatedAmount = Math.Round(amountTobeAllocated * allocation.CoralSpringsAllocation, 2);
        decimal boyntonBeachAllocatedAmount = Math.Round(amountTobeAllocated * allocation.BoyntonBeachAllocation, 2);

        var roundedAmount = coralSpringsAllocatedAmount + boyntonBeachAllocatedAmount + bocaRatonAllocatedAmount;

        if (roundedAmount - amountTobeAllocated > 0)
        {
            bocaRatonAllocatedAmount -= Math.Round((roundedAmount - amountTobeAllocated), 2);
        }
        else if (roundedAmount - amountTobeAllocated < 0)
        {
            bocaRatonAllocatedAmount += Math.Round((amountTobeAllocated - roundedAmount), 2);
        }

        roundedAmount = Math.Round(coralSpringsAllocatedAmount + boyntonBeachAllocatedAmount + bocaRatonAllocatedAmount,2);

        if (roundedAmount != amountTobeAllocated)
        {
            throw new Exception("Amounts do not balance");
        }
        if (coralSpringsAllocatedAmount != 0)
        {
            var payrollEntry = new PayrollEntry
            {
                InvoiceNumber = record.InvoiceNumber,
                Class = "Coral Springs",
                Description = $"{record.Name} ({allocation.CoralSpringsAllocation * 100}% of {record.Amount})",
                Amount = coralSpringsAllocatedAmount
            };
            entries.Add(payrollEntry);
        }

        if (boyntonBeachAllocatedAmount != 0)
        {
            var payrollEntry = new PayrollEntry
            {
                InvoiceNumber = record.InvoiceNumber,
                Class = "Boynton Beach",
                Description = $"{record.Name} ({allocation.BoyntonBeachAllocation * 100}% of {record.Amount})",
                Amount = boyntonBeachAllocatedAmount
            };
            entries.Add(payrollEntry);
        }


        if (bocaRatonAllocatedAmount != 0)
        {
            var payrollEntry = new PayrollEntry
            {
                InvoiceNumber = record.InvoiceNumber,
                Class = "Boca Raton",
                Description = $"{record.Name} ({allocation.BocaRatonAllocation * 100}% of {record.Amount})",
                Amount = bocaRatonAllocatedAmount
            };
            entries.Add(payrollEntry);



        }

    }

}

using (var writer = new StreamWriter($"{workingDirectory}\\output.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(entries);
}
Console.ReadLine();




static Dictionary<string, PayrollAllocation> GetPayrollAllocation(string workingDirectory, string payrollAllocation, CsvConfiguration config)
{
    var payrollAllocationDictionary = new Dictionary<string, PayrollAllocation>();

    using (var reader = new StreamReader($"{workingDirectory}\\{payrollAllocation}"))

    using (var csv = new CsvReader(reader, config))
    {
        var records = csv.GetRecords<PayrollAllocation>();
        foreach (var record in records)
        {
            payrollAllocationDictionary.Add(MakeKey(record), record);
        }
    }

    string MakeKey(PayrollAllocation record)
    {
        return $"{record.Name}-{record.Month.ToString()}";
    }

    return payrollAllocationDictionary;
}
// See https://aka.ms/new-console-template for more information

using Alh.AllocatePayroll;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;



var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Delimiter = ","
};

var payrollAllocationDictionary = new Dictionary<string, PayrollAllocation>();

using (var reader = new StreamReader(@"C:\Users\GaryDavidson\source\repos\Alh.AllocatePayroll\Alh.AllocatePayroll\PayrollAllocTable.csv"))

 
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


var entries = new List<PayrollEntry>();

using (var reader = new StreamReader(@"C:\Users\GaryDavidson\source\repos\Alh.AllocatePayroll\Alh.AllocatePayroll\payrollentries.csv"))
using (var csv = new CsvReader(reader, config))
{
    var records = csv.GetRecords<PayrollDetails>();
    foreach (var record in records)
    {
       var key = $"{record.Name}-{record.PayDate.Month}";
       var allocation = payrollAllocationDictionary[key];

       double allocated = 0;

       if (allocation.CoralSpringsAllocation != 0)
       {
            var payrollEntry = new PayrollEntry();
            payrollEntry.InvoiceNumber = InvoiceNumber(record);
            payrollEntry.Class = "Coral Springs";
            payrollEntry.Description = $"{record.Name} ({allocation.CoralSpringsAllocation * 100}% of {record.Amount})";
            payrollEntry.Amount = Math.Round(double.Parse(record.Amount) * allocation.CoralSpringsAllocation,2);

            entries.Add(payrollEntry);

            if (allocation.CoralSpringsAllocation == 1)
                continue;

            allocated = payrollEntry.Amount;
       }

        if (allocation.BoyntonBeachAllocation != 0)
        {
            var payrollEntry = new PayrollEntry();
            payrollEntry.InvoiceNumber = InvoiceNumber(record);
            payrollEntry.Class = "Boynton Beach";
            payrollEntry.Description = $"{record.Name} ({allocation.BoyntonBeachAllocation * 100}% of {record.Amount})";
           

            if(allocation.BocaRatonAllocation == 0)
            {
                payrollEntry.Amount = double.Parse(record.Amount) - allocated;
            }
            else
            {
                payrollEntry.Amount = Math.Round(double.Parse(record.Amount) * allocation.BoyntonBeachAllocation);
                
            }
            allocated += payrollEntry.Amount;
            entries.Add(payrollEntry);

            if (allocation.BoyntonBeachAllocation == 1)
                continue;
        }
        

        if (allocation.BocaRatonAllocation != 0)
        {
            var payrollEntry = new PayrollEntry();
            payrollEntry.InvoiceNumber = InvoiceNumber(record);
            payrollEntry.Class = "Boca Raton";
            payrollEntry.Description = $"{record.Name} ({allocation.BocaRatonAllocation * 100}% of {record.Amount})";
            payrollEntry.Amount = double.Parse(record.Amount) - allocated; 
            allocated += payrollEntry.Amount;
            entries.Add(payrollEntry);


            if (allocation.BocaRatonAllocation == 1)
                continue;
        }

        if(double.Parse(record.Amount) - allocated != 0)
        {
            throw new Exception("Allocated amount does not equal payroll amount");
        }
    }

}

using (var writer = new StreamWriter($"{Environment.CurrentDirectory}\\output.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(entries);
}


string InvoiceNumber(PayrollDetails detail)
{

    return $"{detail.PayDate.Year}-{detail.PayDate.Month }-{detail.PayDate.Day }";

}

Console.ReadLine();
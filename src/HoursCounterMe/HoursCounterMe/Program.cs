// 12:38 pm 12Dec2022
const string filePath = "count.txt";

Console.WriteLine("Please, input data in this format HH:MM am/pm DDMMMYYYY");
var consoleInput = Console.ReadLine();

if (consoleInput!.Equals(string.Empty))
{
    Console.WriteLine("Incorrect input!");
}

var isParsed = DateTime.TryParse(consoleInput, result: out var dateTime);

if (!isParsed)
{
    Console.WriteLine("Incorrect format!");
}

var hoursToFile = dateTime.Hour;
var minutesToFile = dateTime.Minute;

if (!File.Exists(filePath))
{ 
    File.Create(filePath);
}

var textInFile = File.ReadAllText(filePath);

if (textInFile.Length == 0)
{
    File.WriteAllText(filePath, $"{hoursToFile}:{minutesToFile}");

    return;
}

var splitDataFromFile = textInFile.Split(':');
var hoursFromFile = int.Parse(splitDataFromFile[0]);
var minutesFromFile = int.Parse(splitDataFromFile[1]);

var dateForSum = DateTime.MinValue;
var minutesToSum = dateForSum.AddMinutes(minutesToFile + minutesFromFile).Minute;
var hoursToSum = hoursFromFile + hoursToFile;

File.WriteAllText(filePath, $"{hoursToSum}:{minutesToSum}");
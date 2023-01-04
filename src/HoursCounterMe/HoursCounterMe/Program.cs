//12:38 pm 12Dec2022

using System.Text.RegularExpressions;

const string countFilePath = "count.txt";
const string currentFilePath = "current.txt";
const string historyFilePath = "history.txt";
const string pattern = @"^\d{2}:\d{2} (am|pm) \d{2}[A-Za-z]{3}\d{4}$";

InitFiles(new[] { countFilePath, currentFilePath, historyFilePath });

while (true)
{
    Console.WriteLine("Choose action:");
    Console.WriteLine("1 - Add/finish current session");
    Console.WriteLine("2 - Show current session");
    Console.WriteLine("3 - Show history of sessions");
    Console.WriteLine("4 - Show hours count");
    Console.WriteLine("5 - Add count data manually");
    Console.WriteLine("6 - Exit");
    var userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            ProcessCurrentSession();
            break;
        case "2":
            ShowCurrentSession();
            break;
        case "3":
            ShowSessionsHistory();
            break;
        case "6":
            Console.WriteLine("Bye-bye!");
            return;
        default:
            Console.WriteLine("Invalid input!");
            break;
    }
}

void ProcessCurrentSession()
{
    var fileData = File.ReadAllText(currentFilePath);

    if (fileData.Equals(string.Empty))
        try
        {
            var currentSessionFirstPart = GetCurrentSessionPart(true);
            File.WriteAllText(currentFilePath, currentSessionFirstPart);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    else
        // ToDo: verify that prev stored part of current session in file is in correct format
        try
        {
            var currentSessionLastPart = GetCurrentSessionPart(false);
            var currentSessionFirstPart = File.ReadAllText(currentFilePath);
            var fullSession = $"{currentSessionFirstPart} - {currentSessionLastPart}";
            File.AppendAllText(historyFilePath, $"\n{fullSession}");
            File.WriteAllText(currentFilePath, string.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
}

string GetCurrentSessionPart(bool isFirstPartOfSession)
{
    Console.WriteLine(isFirstPartOfSession
        ? "Input a first part of the current session in this format: HH:MM am/pm DDMMMYYYY"
        : "Input a last part of the current session in this format: HH:MM am/pm DDMMMYYYY");
    var userInput = Console.ReadLine();

    if (userInput!.Equals(string.Empty)) throw new Exception("Empty input!");

    if (!Regex.IsMatch(userInput, pattern))
        throw new Exception("Incorrect input format! Format: HH:MM am/pm DDMMMYYYY");

    return userInput;
}

void ShowCurrentSession()
{
    var fileData = File.ReadAllText(currentFilePath);
    var result = fileData.Equals(string.Empty) ? "You don't have a current session" : fileData;
    Console.WriteLine(result);
}

void ShowSessionsHistory()
{
    File.ReadAllLines(historyFilePath).ToList().ForEach(Console.WriteLine);
}

void InitFiles(IEnumerable<string> filesPaths)
{
    filesPaths.ToList().ForEach(filePath =>
    {
        if (!File.Exists(filePath)) File.Create(filePath);
    });
}

#region old

// Console.WriteLine("Please, input data in this format HH:MM am/pm DDMMMYYYY");
// var consoleInput = Console.ReadLine();
//
// if (consoleInput!.Equals(string.Empty))
// {
//     Console.WriteLine("Incorrect input!");
// }
//
// var isParsed = DateTime.TryParse(consoleInput, result: out var dateTime);
//
// if (!isParsed)
// {
//     Console.WriteLine("Incorrect format!");
// }
//
// var hoursToFile = dateTime.Hour;
// var minutesToFile = dateTime.Minute;
//
// if (!File.Exists(filePath))
// { 
//     File.Create(filePath);
// }
//
// var textInFile = File.ReadAllText(filePath);
//
// if (textInFile.Length == 0)
// {
//     File.WriteAllText(filePath, $"{hoursToFile}:{minutesToFile}");
//
//     return;
// }
//
// var splitDataFromFile = textInFile.Split(':');
// var hoursFromFile = int.Parse(splitDataFromFile[0]);
// var minutesFromFile = int.Parse(splitDataFromFile[1]);
//
// var dateForSum = DateTime.MinValue;
// var minutesToSum = dateForSum.AddMinutes(minutesToFile + minutesFromFile).Minute;
// var hoursToSum = hoursFromFile + hoursToFile;
//
// File.WriteAllText(filePath, $"{hoursToSum}:{minutesToSum}");

#endregion
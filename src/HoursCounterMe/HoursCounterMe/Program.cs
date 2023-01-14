using System.Text.RegularExpressions;

const string countFilePath = "count.txt";
const string currentFilePath = "current.txt";
const string historyFilePath = "history.txt";
const string mainDataPattern = @"^\d{2}:\d{2} (am|pm) \d{2}[A-Za-z]{3}\d{4}$";

InitFiles(new[] { countFilePath, currentFilePath, historyFilePath });

while (true)
{
    Console.WriteLine("Choose action:");
    Console.WriteLine("1 - Add/finish current session");
    Console.WriteLine("2 - Show current session");
    Console.WriteLine("3 - Show history of sessions");
    Console.WriteLine("4 - Add count data manually");
    Console.WriteLine("5 - Show hours count");
    Console.WriteLine("6 - Create backup");
    Console.WriteLine("7 - Import backup");
    Console.WriteLine("8 - Exit");
    var userInput = Console.ReadLine();

    switch (userInput)
    {
        case "1":
            CreateCurrentSession();
            break;
        case "2":
            ShowCurrentSession();
            break;
        case "3":
            ShowSessionsHistory();
            break;
        case "4":
            AddCountDataManually();
            Console.WriteLine(File.ReadAllText(countFilePath));
            break;
        case "5":
            Console.WriteLine(File.ReadAllText(countFilePath));
            break;
        case "6":
            CreateBackup();
            break;
        case "7":
            ImportBackup();
            break;
        case "8":
            Console.WriteLine("Bye-bye!");
            return;
        default:
            Console.WriteLine("Invalid input!");
            break;
    }
}

void ImportBackup()
{
    Console.WriteLine("Input backup directory path:");
    var backupDirPath = Console.ReadLine();

    if (backupDirPath == null || backupDirPath.Equals(string.Empty))
    {
        Console.WriteLine("Backup directory path is null or empty!");
    }
    else
    {
        if (File.Exists(countFilePath))
        {
            File.Delete(countFilePath);
        }

        if (File.Exists(currentFilePath))
        {
            File.Delete(currentFilePath);
        }

        if (File.Exists(historyFilePath))
        {
            File.Delete(historyFilePath);
        }

        var countFileBackupPath = backupDirPath + @"\count.txt";
        var currentFileBackupPath = backupDirPath + @"\current.txt";
        var historyFileBackupPath = backupDirPath + @"\history.txt";

        try
        {
            File.Copy(countFileBackupPath, countFilePath);
            File.Copy(currentFileBackupPath, currentFilePath);
            File.Copy(historyFileBackupPath, historyFilePath);
            Console.WriteLine("Successfully imported files.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

void CreateBackup()
{
    Console.WriteLine("Input backup directory path:");
    var backupDirPath = Console.ReadLine();

    if (backupDirPath == null || backupDirPath.Equals(string.Empty))
    {
        Console.WriteLine("Backup directory path is null or empty!");
    }
    else
    {
        var countFileBackupPath = backupDirPath + @"\count.txt";
        var currentFileBackupPath = backupDirPath + @"\current.txt";
        var historyFileBackupPath = backupDirPath + @"\history.txt";

        try
        {
            File.Copy(countFilePath, countFileBackupPath);
            File.Copy(currentFilePath, currentFileBackupPath);
            File.Copy(historyFilePath, historyFileBackupPath);
            Console.WriteLine("Successfully copied.");
        }
        catch (IOException)
        {
            File.Delete(countFileBackupPath);
            File.Delete(currentFileBackupPath);
            File.Delete(historyFileBackupPath);
            File.Copy(countFilePath, countFileBackupPath);
            File.Copy(currentFilePath, currentFileBackupPath);
            File.Copy(historyFilePath, historyFileBackupPath);
            Console.WriteLine("Successfully deleted previous version of file and copied.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

void AddCountDataManually()
{
    Console.WriteLine("Input data in this format HH:MM");
    var timeToAddInString = Console.ReadLine();

    if (timeToAddInString == null || !Regex.IsMatch(timeToAddInString, @"^\d{2}:\d{2}"))
    {
        Console.WriteLine("Your data was null or in wrong format!");
    }
    else
    {
        var separatedStrings = timeToAddInString.Split(':');
        var hoursFromCountFile = int.Parse(separatedStrings[0]);
        var minutesFromCountFile = int.Parse(separatedStrings[1]);
        var timeToAdd = new TimeSpan(hoursFromCountFile, minutesFromCountFile, 0);
        AddCountData(timeToAdd);
    }
}

void CreateCurrentSession()
{
    var fileData = File.ReadAllText(currentFilePath);

    if (fileData.Equals(string.Empty))
    {
        try
        {
            var currentSessionFirstPart = GetCurrentSessionPart(true);
            File.WriteAllText(currentFilePath, currentSessionFirstPart);
            Console.WriteLine("Successfully added first part of the current session.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    else
    {
        try
        {
            var currentSessionLastPart = GetCurrentSessionPart(false);
            var currentSessionFirstPart = File.ReadAllText(currentFilePath);
            CountSessionHoursAndMinutes(currentSessionFirstPart, currentSessionLastPart);
            var fullSession = $"{currentSessionFirstPart} - {currentSessionLastPart}";
            File.AppendAllText(historyFilePath, $"\n{fullSession}");
            File.WriteAllText(currentFilePath, string.Empty);
            Console.WriteLine("Successfully added second part of the current session and finished it.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

void CountSessionHoursAndMinutes(string sessionFirstPart, string sessionLastPart)
{
    var isParsed = DateTime.TryParse(sessionFirstPart, out var sessionFirstPartDateTime);
    if (!isParsed)
    {
        throw new Exception("Invalid format of session's first part read from current session file!");
    }

    isParsed = DateTime.TryParse(sessionLastPart, out var sessionLastPartDateTime);
    if (!isParsed)
    {
        throw new Exception("Invalid format of session's last part read from console!");
    }

    var timeToCount = sessionLastPartDateTime.Subtract(sessionFirstPartDateTime);
    var timeFromCountFileString = File.ReadAllText(countFilePath);
    if (!timeFromCountFileString.Equals(string.Empty))
    {
        var separatedStrings = timeFromCountFileString.Split(':');
        var hoursFromCountFile = int.Parse(separatedStrings[0]);
        var minutesFromCountFile = int.Parse(separatedStrings[1]);
        var timeFromCountFile = new TimeSpan(hoursFromCountFile, minutesFromCountFile, 0);

        timeToCount += timeFromCountFile;
    }

    var hours = timeToCount.Days * 24 + timeToCount.Hours;
    var minutes = timeToCount.Minutes;
    File.WriteAllText(countFilePath, $"{hours}:{minutes}");
}

void AddCountData(TimeSpan timeToAdd)
{
    var timeFromCountFileString = File.ReadAllText(countFilePath);
    if (!timeFromCountFileString.Equals(string.Empty))
    {
        var separatedStrings = timeFromCountFileString.Split(':');
        var hoursFromCountFile = int.Parse(separatedStrings[0]);
        var minutesFromCountFile = int.Parse(separatedStrings[1]);
        var timeFromCountFile = new TimeSpan(hoursFromCountFile, minutesFromCountFile, 0);

        timeToAdd += timeFromCountFile;
    }

    var hours = timeToAdd.Days * 24 + timeToAdd.Hours;
    var minutes = timeToAdd.Minutes;
    File.WriteAllText(countFilePath, $"{hours}:{minutes}");
}

string GetCurrentSessionPart(bool isFirstPartOfSession)
{
    Console.WriteLine(isFirstPartOfSession
        ? "Input a first part of the current session in this format: HH:MM am/pm DDMMMYYYY"
        : "Input a last part of the current session in this format: HH:MM am/pm DDMMMYYYY");
    var userInput = Console.ReadLine();

    if (userInput!.Equals(string.Empty))
    {
        throw new Exception("Empty input!");
    }

    if (!Regex.IsMatch(userInput, mainDataPattern))
    {
        throw new Exception("Incorrect input format! Format: HH:MM am/pm DDMMMYYYY");
    }

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
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
    });
}
using Avtotest.Data.Models;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

var connection = new SqliteConnection("Data source=avtotest.db");
connection.Open();

var command = connection.CreateCommand();

CreateTable();

void CreateTable()
{
    command.CommandText = "CREATE TABLE IF NOT EXISTS " +
                          "questions(id INTEGER PRIMARY KEY AUTOINCREMENT, question_text TEXT , description TEXT, media TEXT)";
    command.ExecuteNonQuery();

    command.CommandText = "CREATE TABLE IF NOT EXISTS " +
                          "choices(id INTEGER PRIMARY KEY AUTOINCREMENT, text TEXT, answer BOOLEAN, questionId INTEGER)";
    command.ExecuteNonQuery();
}



var jsonData = File.ReadAllText(@"D:\Projects\WriteQuestionsJsonDataToDB\WriteQuestionsJsonDataToDB\WriteQuestionsJsonDataToDB\JsonData\uzlotin.json");
var questionsEntities = JsonConvert.DeserializeObject<List<QuestionEntity>>(jsonData);

if (questionsEntities is null)
{
    Console.WriteLine("questionsEntities is null");
    return;
}

foreach (var questionEntity in questionsEntities)
{
    var sqlMedia = questionEntity.Media!.Exist ? $"{questionEntity.Media.Name}.png" : "noimage.png"; 

    command.CommandText = "INSERT INTO questions (id, question_text, description, media) " +
                          $"VALUES ({questionEntity.Id}, \"{questionEntity.Question}\", \"{questionEntity.Description}\", \"{sqlMedia}\")";
    command.ExecuteNonQuery();

    AddChoices(questionEntity.Choices!, questionEntity.Id);
}

void AddChoices(List<Choice> questionEntityChoices, int questionId)
{
    foreach (var questionEntityChoice in questionEntityChoices)
    {
        command.CommandText = "INSERT INTO choices (text, answer, questionId) " +
                              $"VALUES (\"{questionEntityChoice.Text}\", {questionEntityChoice.Answer}, {questionId})";
        command.ExecuteNonQuery();
    }
}
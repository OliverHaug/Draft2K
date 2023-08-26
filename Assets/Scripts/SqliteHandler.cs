using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class SqliteHandler : MonoBehaviour
{
    void Awake()
    {
        if (!DatabaseHasData())
        {
            AddFormations();
        }

    }

    private bool DatabaseHasData()
    {
        using (IDbConnection dbConnection = CreateAndOpenDatabase())
        {
            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Formation";
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }

    private void AddFormations()
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();
        TextAsset jsonAsset = Resources.Load<TextAsset>("formations");
        if (jsonAsset == null)
        {
            Debug.LogError("JSON-Datei konnte nicht geladen werden!");
            return;
        }

        JsonFormations loadedFormations = JsonUtility.FromJson<JsonFormations>(jsonAsset.text);

        foreach (JsonFormation jsonFormation in loadedFormations.formations)
        {
            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "INSERT OR IGNORE INTO Formation (formationName, images) VALUES (@formationName, @images)";
                cmd.Parameters.Add(new SqliteParameter("@formationName", jsonFormation.formationName));
                cmd.Parameters.Add(new SqliteParameter("@images", jsonFormation.images));
                cmd.ExecuteNonQuery();
            }

            foreach (JsonPositionWrapper jsonPosWrapper in jsonFormation.positions)
            {
                using (IDbCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = "INSERT OR IGNORE INTO Position (formationName, positionName, leftPosition, topPosition) VALUES (@formationName, @positionName, @leftPosition, @topPosition)";
                    cmd.Parameters.Add(new SqliteParameter("@formationName", jsonFormation.formationName));
                    cmd.Parameters.Add(new SqliteParameter("@positionName", jsonPosWrapper.positionName));
                    cmd.Parameters.Add(new SqliteParameter("@leftPosition", jsonPosWrapper.position.left));
                    cmd.Parameters.Add(new SqliteParameter("@topPosition", jsonPosWrapper.position.top));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        dbConnection.Close();
    }

    public List<Formation> LoadAllFormations()
    {
        List<Formation> formations = new List<Formation>();

        using (IDbConnection dbConnection = CreateAndOpenDatabase())
        {
            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Formation";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Formation formation = new Formation
                        {
                            formationName = reader.GetString(0),
                            images = reader.GetString(1),
                            positions = LoadPositionsForFormation(reader.GetString(0), dbConnection)
                        };
                        formations.Add(formation);
                    }
                }
            }
        }

        return formations;
    }

    private List<Position> LoadPositionsForFormation(string formationName, IDbConnection dbConnection)
    {
        List<Position> positions = new List<Position>();

        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM Position WHERE formationName = @formationName";
            IDbDataParameter param = cmd.CreateParameter();
            param.ParameterName = "@formationName";
            param.Value = formationName;
            cmd.Parameters.Add(param);

            using (IDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    float xPercentage = float.Parse(reader.GetString(3).TrimEnd('%'));
                    float yPercentage = float.Parse(reader.GetString(4).TrimEnd('%'));

                    Position position = new Position
                    {
                        positionName = reader.GetString(2),
                        position = new Vector2(xPercentage, yPercentage)
                    };
                    positions.Add(position);
                }
            }
        }

        return positions;
    }

    private IDbConnection CreateAndOpenDatabase()
    {
        string dbUri = "URI=file:Draft2k.sqlite";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        // Player Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Player (id TEXT PRIMARY KEY, image TEXT, name TEXT, cardId TEXT, nationId TEXT, clubId TEXT)";
            cmd.ExecuteNonQuery();
        }

        // Attributes Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Attributes (playerId TEXT, attribute1 INTEGER, attribute2 INTEGER, attribute3 INTEGER, attribute4 INTEGER, attribute5 INTEGER, attribute6 INTEGER, rating INTEGER, position TEXT, altPosition TEXT, FOREIGN KEY(playerId) REFERENCES Player(id))";
            cmd.ExecuteNonQuery();
        }

        // Card Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Card (id TEXT PRIMARY KEY, image TEXT, name TEXT, quality TEXT, overlayBackgroundColor TEXT, overlayDividerColor TEXT, overlayTextColor TEXT)";
            cmd.ExecuteNonQuery();
        }

        // Nation Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Nation (id TEXT PRIMARY KEY, image TEXT, name TEXT)";
            cmd.ExecuteNonQuery();
        }

        // League Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS League (id TEXT PRIMARY KEY, image TEXT, name TEXT, nationId TEXT, FOREIGN KEY(nationId) REFERENCES Nation(id))";
            cmd.ExecuteNonQuery();
        }

        // Club Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Club (id TEXT PRIMARY KEY, image TEXT, name TEXT, leagueId TEXT, FOREIGN KEY(leagueId) REFERENCES League(id))";
            cmd.ExecuteNonQuery();
        }

        // Formation Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Formation (formationName TEXT PRIMARY KEY, images TEXT)";
            cmd.ExecuteNonQuery();
        }

        // Position Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Position (id INTEGER PRIMARY KEY AUTOINCREMENT, formationName TEXT, positionName TEXT, leftPosition TEXT, topPosition TEXT, FOREIGN KEY(formationName) REFERENCES Formation(formationName))";
            cmd.ExecuteNonQuery();
        }

        return dbConnection;
    }


}

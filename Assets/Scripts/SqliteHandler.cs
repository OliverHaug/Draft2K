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

        Formations loadedFormations = JsonUtility.FromJson<Formations>(jsonAsset.text);

        foreach (Formation formation in loadedFormations.formations)
        {
            using (IDbCommand cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "INSERT OR IGNORE INTO Formation (formationName, images) VALUES (@formationName, @images)";
                cmd.Parameters.Add(new SqliteParameter("@formationName", formation.formationName));
                cmd.Parameters.Add(new SqliteParameter("@images", formation.images));
                cmd.ExecuteNonQuery();
            }

            foreach (Position pos in formation.positions)
            {
                using (IDbCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = "INSERT OR IGNORE INTO Position (formationName, positionName, xPosition, yPosition) VALUES (@formationName, @positionName, @xPosition, @yPosition)";
                    cmd.Parameters.Add(new SqliteParameter("@formationName", formation.formationName));
                    cmd.Parameters.Add(new SqliteParameter("@positionName", pos.positionName));
                    cmd.Parameters.Add(new SqliteParameter("@xPosition", DbType.Double) { Value = pos.x });
                    cmd.Parameters.Add(new SqliteParameter("@yPosition", DbType.Double) { Value = pos.y });
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
                    float xPosition = reader.GetFloat(reader.GetOrdinal("xPosition"));
                    float yPosition = reader.GetFloat(reader.GetOrdinal("yPosition"));
                    Position position = new Position
                    {
                        positionName = reader.GetString(2),
                        x = xPosition,
                        y = yPosition,
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

        // Formation Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Formation (formationName TEXT PRIMARY KEY, images TEXT)";
            cmd.ExecuteNonQuery();
        }

        // Position Table
        using (IDbCommand cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Position (id INTEGER PRIMARY KEY AUTOINCREMENT, formationName TEXT, positionName TEXT, xPosition REAL, yPosition REAL, FOREIGN KEY(formationName) REFERENCES Formation(formationName))";
            cmd.ExecuteNonQuery();
        }

        return dbConnection;
    }
}

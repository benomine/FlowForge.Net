using Npgsql;

namespace FlowForge.Net;

/// <inheritdoc />
public class NpgsqlJobStepRepository : IJobStepRepository
{
    private readonly NpgsqlDataSource? _dataSource;
    
    internal NpgsqlJobStepRepository()
    {
        
    }

    /// <inheritdoc cref="NpgsqlJobStepRepository" />
    public NpgsqlJobStepRepository(string connectionString)
    {
        _dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
    }
    
    /// <inheritdoc cref="NpgsqlJobStepRepository"/>
    public NpgsqlJobStepRepository(NpgsqlDataSource? dataSource)
    {
        _dataSource = dataSource;
    }
    
    /// <inheritdoc />
    public virtual int SaveStep(IStep step)
    {
        using var connection = _dataSource!.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
              INSERT INTO Steps (JobId, StepId, Name, Status, StartTime) 
              VALUES (@JobId, @StepId, @Name, @Status, CURRENT_TIMESTAMP)
            """;
        command.Parameters.AddWithValue("JobId", step.JobId);
        command.Parameters.AddWithValue("StepId", step.StepId);
        command.Parameters.AddWithValue("Name", step.Name);
        command.Parameters.AddWithValue("Status", step.Status.ToString());
        return command.ExecuteNonQuery();
    }
    
    /// <inheritdoc />
    public virtual int UpdateStep(IStep step)
    {
        using var connection = _dataSource!.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
              UPDATE Steps set Status = @Status, Message = @Message, Exception = @Exception, EndTime = @EndTime,
              UpdatedAt = CURRENT_TIMESTAMP where StepId = @StepId and @JobId = @JobId
            """;
        command.Parameters.AddWithValue("JobId", step.Name);
        command.Parameters.AddWithValue("StepId", step.StepId);
        command.Parameters.AddWithValue("Name", step.Name);
        command.Parameters.AddWithValue("Status", step.Status.ToString());
        command.Parameters.AddWithValue("EndTime", step.EndTime);
        if (step.Message is not null)
        {
            command.Parameters.AddWithValue("Message", step.Message);
        }
        else
        {
            command.Parameters.AddWithValue("Message", DBNull.Value);
        }
        if (step.Exception is not null)
        {
            command.Parameters.AddWithValue("Exception", step.Exception);
        }
        else
        {
            command.Parameters.AddWithValue("Exception", DBNull.Value);
        }
        return command.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public List<IStep> GetStepsById<T>(Guid jobId, Guid stepId) where T : IStep, new()
    {
        using var connection = _dataSource!.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT JobId, StepId, Name, Status, CreatedAt, UpdatedAt, StartTime, EndTime, Message, Exception
            from Steps where JobId = @JobId and StepId = @StepId;"
            """;
        command.Parameters.AddWithValue("JobId", jobId);
        command.Parameters.AddWithValue("StepId", stepId);
        using var reader = command.ExecuteReader();
        var steps = new List<IStep>();
        while (reader.Read())
        {
            steps.Add(new T
            {
                JobId = reader.GetGuid(0),
                StepId = reader.GetGuid(1),
                Name = reader.GetString(2),
                Status = Enum.Parse<StepStatus>(reader.GetString(2)),
                CreatedAt = reader.GetDateTime(3),
                UpdatedAt = reader.GetDateTime(4),
                StartTime = reader.GetDateTime(5),
                EndTime = reader.GetDateTime(6),
                Message = reader.GetString(7),
                Exception = reader.GetString(8)
            });
        }

        return steps;
    }

    /// <inheritdoc />
    public List<IStep> GetStepsByName<T>(Guid jobId, string stepName) where T : IStep, new()
    {
        using var connection = _dataSource!.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT JobId, StepId, Name, Status, CreatedAt, UpdatedAt, StartTime, EndTime, Message, Exception
            from Steps where JobId = @JobId and Name = @Name;"
            """;
        command.Parameters.AddWithValue("JobId", jobId);
        command.Parameters.AddWithValue("Name", stepName);
        using var reader = command.ExecuteReader();
        var steps = new List<IStep>();
        while (reader.Read())
        {
            steps.Add(new T
            {
                JobId = reader.GetGuid(0),
                StepId = reader.GetGuid(1),
                Name = reader.GetString(2),
                Status = Enum.Parse<StepStatus>(reader.GetString(2)),
                CreatedAt = reader.GetDateTime(3),
                UpdatedAt = reader.GetDateTime(4),
                StartTime = reader.GetDateTime(5),
                EndTime = reader.GetDateTime(6),
                Message = reader.GetString(7),
                Exception = reader.GetString(8)
            });
        }

        return steps;
    }

    /// <inheritdoc />
    public void Init()
    {
        using var connection = _dataSource!.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Jobs (
                JobId UUID PRIMARY KEY,
                JobName VARCHAR(255) NOT NULL,
                Status VARCHAR(50) NOT NULL,
                CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );
            CREATE TABLE IF NOT EXISTS Steps (
                JobId UUID NOT NULL,
                StepId UUID NOT NULL,
                Name VARCHAR(255) NOT NULL,
                Status VARCHAR(50) NOT NULL,
                StartTime TIMESTAMP DEFAULT NULL,
                EndTime TIMESTAMP DEFAULT NULL,
                Message TEXT,
                Exception TEXT,
                CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (JobId, StepId),
                FOREIGN KEY (JobId) REFERENCES Jobs(JobId)
            );
            """;
        command.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void SaveJob(Job job)
    {
        using var connection = _dataSource!.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO Jobs (JobId, JobName, Status) VALUES (@JobId, @JobName, @Status)
            """;
        command.Parameters.AddWithValue("JobId", job.JobId);
        command.Parameters.AddWithValue("JobName", job.JobName);
        command.Parameters.AddWithValue("Status", JobStatus.Started.ToString());
        command.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public void UpdateJob(Job job)
    {
        using var connection = _dataSource!.CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE Jobs SET Status = @Status, UpdatedAt = CURRENT_TIMESTAMP WHERE JobId = @JobId
            """;
        command.Parameters.AddWithValue("JobId", job.JobId);
        command.Parameters.AddWithValue("Status", job.Status.ToString());
        command.ExecuteNonQuery();
    }
}

﻿using ConsoleTables;
using Expresso.Components;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.Data.Sqlite;
using Parcel.CoreEngine.Helpers;
using Parcel.Types;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Expresso.Components
{
    #region Main DB Definition
    public enum DestinationDatabase
    {
        InMemoryDB,
        Oracle
    }
    public class InMemorySQLIte
    {
        #region Construction
        protected SqliteConnection DB { get; private set; }
        public InMemorySQLIte()
        {
            InitializeInMemoryDB();

            void InitializeInMemoryDB()
            {
                DB = new SqliteConnection("Data Source=:memory:");
                DB.Open();
                // InMemoryDB.Close(); // We never do this
            }
        }
        #endregion

        #region interface
        public DataTable Execute(string sqlQuery)
        {
            try
            {
                if (sqlQuery.ToLower().Trim().StartsWith("select"))
                {
                    string formattedQuery = sqlQuery.EndsWith(';') ? sqlQuery : sqlQuery + ';';
                    DataTable table = new();
                    table.Load(new SqliteCommand(formattedQuery, DB).ExecuteReader());
                    return table;
                }
                else
                {
                    using var command = new SqliteCommand(sqlQuery, DB);
                    command.ExecuteNonQuery();
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message.Replace(Environment.NewLine, " "));
            }
        }
        public DataGrid RunSQL(string sqlQuery)
        {
            try
            {
                if (sqlQuery.ToLower().Trim().StartsWith("select"))
                {
                    string formattedQuery = sqlQuery.EndsWith(';') ? sqlQuery : sqlQuery + ';';
                    DataTable table = new();
                    table.Load(new SqliteCommand(formattedQuery, DB).ExecuteReader());
                    return new DataGrid(table);
                }
                else
                {
                    using var command = new SqliteCommand(sqlQuery, DB);
                    command.ExecuteNonQuery();
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message.Replace(Environment.NewLine, " "));
            }
        }
        public static void Display(DataGrid dataGrid)
        {
            Console.WriteLine(string.Join(',', dataGrid.Columns.Select(c => c.Header)), Color.Orange);
            // Display with empty trailing line only if there is no data
            string csv = dataGrid.ToCSV(false);
            int lineCount = Regex.Matches(csv, Environment.NewLine).Count;
            if (lineCount == 1) Console.WriteLine(csv);
            else Console.WriteLine(csv.TrimEnd());
        }
        public void Import(IEnumerable<string[]> csvLines, string[] headers, string tableName, out DataTable dataTable)
        {
            SqliteCommand cmd = DB.CreateCommand();
            cmd.CommandText = $"CREATE TABLE '{tableName}'({string.Join(',', headers.Select(c => $"\"{c}\""))})";
            cmd.ExecuteNonQuery();

            InsertDbData(DB, tableName, new DataGrid(tableName, csvLines, headers));
            dataTable = Execute($"select * from {tableName}");
        }
        public void Import(DataTable dataTable, string tableName)
        {
            if (dataTable.Rows.Count == 0)
            {
                DataGrid emptyGrid = new();
                foreach (System.Data.DataColumn col in dataTable.Columns)
                    emptyGrid.AddColumn(col.ColumnName);
                Import(emptyGrid, tableName);
            }
            else
            {
                // Convert to CSV (we could do better, but this is easy)
                string csv = dataTable.ToCSV();

                // Import as csv
                Import(CSVHelper.ParseCSV(csv, out string[] headers, true), headers, tableName, out _); // TODO: Remark-cz: Why can't we directly import from DataGrid?
            }
        }
        public void Import(DataGrid table, string tableName)
        {
            SqliteCommand cmd = DB.CreateCommand();
            cmd.CommandText = $"CREATE TABLE '{tableName}'({string.Join(',', table.Columns.Select(c => $"\"{c.Header}\""))})";
            cmd.ExecuteNonQuery();

            InsertDbData(DB, tableName, table);
        }
        public void ImportFromODBC(string databaseName, string query, string destinationTable)
        {
            Import(FetchFromODBCDatabase(databaseName, query), destinationTable);
        }
        /// <summary>
        /// Migrate table from current InMemoryDB to another; If target table already exists, simply append to it.
        /// </summary>
        public void Migrate(InMemorySQLIte other, string tableName, string targetTableName, string dsn)
        {
            if (!TablesAndViews.Contains(tableName))
                throw new ArgumentException($"Table '{tableName}' doesn't exist.");
            if (other.Tables.Contains(targetTableName))
                other.Push(DestinationDatabase.InMemoryDB, targetTableName, this[tableName], dsn);
            else
                other.Import(this[tableName], targetTableName);
        }
        /// <summary>
        /// Append content of in-memory table to Oracle table. 
        /// </summary>
        public void Transfer(string tableName, string dsn, string remoteTableName = null)
        {
            remoteTableName ??= tableName;
            var table = this[tableName];
            Push(DestinationDatabase.Oracle, remoteTableName, table, dsn);
        }
        /// <summary>
        /// Append content of a dataframe to target database.
        /// </summary>
        public void Push(DestinationDatabase target, string tableName, DataGrid dataframe, string dsn)
        {
            if (target == DestinationDatabase.InMemoryDB)
                InsertDbData(DB, tableName, dataframe);
            else if (target == DestinationDatabase.Oracle)
                InsertODBCData(tableName, dataframe, dsn);
            else
                throw new ArgumentException($"Invalid target: Target should be either `{DestinationDatabase.InMemoryDB}` or `{DestinationDatabase.Oracle}.");
        }
        internal void Export(string path = "export.sqlite")
        {
            SqliteConnection saveFile = new($"Data Source={path}");
            saveFile.Open();
            DB.BackupDatabase(saveFile, "main", "main");
            saveFile.Close();
        }
        #endregion

        #region Helpers
        public static DataGrid FetchFromODBCDatabase(string query, string dsn)
        {
            OdbcConnection odbcConnection = new($"DSN={dsn}");
            odbcConnection.Open();
            DataTable dt = new();
            dt.Load(new OdbcCommand(query, odbcConnection).ExecuteReader());

            // Convert to CSV
            StringBuilder output = new();
            output.AppendLine(string.Join(",", Enumerable.Range(0, dt.Columns.Count).Select(i => dt.Columns[i].ColumnName)));
            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> items = row.ItemArray.Select(item => $"\"{item.ToString().Replace("\"", "\"\"")}\"");
                string csvLine = string.Join(",", items);
                output.AppendLine(csvLine);
            }
            string csv = output.ToString();

            // Convert to Datagrid
            return new DataGrid("Unnamed", CSVHelper.ReadCSVFile(csv, out string[] headers, true), headers);
        }
        public static void InsertODBCData(string tableName, DataGrid table, string dsn)
        {
            OdbcConnection odbcConnection = new($"DSN={dsn}");
            odbcConnection.Open();
            InsertDbData(odbcConnection, tableName, table);
            odbcConnection.Close();
        }
        public static DataTable ExecuteODBC(string sqlQuery, string dsn)
        {
            OdbcConnection odbcConnection = new($"DSN={dsn}");
            odbcConnection.Open();
            try
            {
                if (sqlQuery.ToLower().Trim().StartsWith("select"))
                {
                    DataSet result = new();
                    string formattedQuery = sqlQuery.EndsWith(';') ? sqlQuery : sqlQuery + ';';
                    using var adapter = new OdbcDataAdapter(formattedQuery, odbcConnection);

                    adapter.Fill(result);
                    return result.Tables[0];
                }
                else
                {
                    using var command = new OdbcCommand(sqlQuery, odbcConnection);
                    command.ExecuteNonQuery();
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message.Replace(Environment.NewLine, " "));
            }
            finally
            {
                odbcConnection.Close();
            }
        }
        public static DataTable ExecuteMDXQuery(string dataSource, string mdxQuery)
        {
            using AdomdConnection conn = new(@$"Data Source={dataSource};Initial Catalog=RiskCube;Provider=MSOLAP.5;Integrated Security=SSPI;Format=Tabular");
            conn.Open();

            DataTable datatable = new AdomdCommand(mdxQuery, conn).ExecuteCellSet().CellSetToTable();
            return datatable;
        }
        #endregion

        #region Accessor
        public IEnumerable<string> Views => RunSQL(@"SELECT name FROM sqlite_schema WHERE type='view' ORDER BY name").Columns[0].GetDataAs<string>();
        public IEnumerable<string> Tables => RunSQL(@"SELECT name FROM sqlite_schema WHERE type='table' ORDER BY name").Columns[0].GetDataAs<string>();
        public IEnumerable<string> TablesAndViews => RunSQL(@"SELECT name FROM sqlite_schema WHERE type in ('table', 'view') ORDER BY name").Columns[0].GetDataAs<string>();
        public DataGrid GetDataGrid(string name)
        {
            return RunSQL(@$"select * from {name}");
        }
        public DataTable GetDataTable(string name)
        {
            return Execute(@$"select * from {name}");
        }
        public DataGrid this[string name] => GetDataGrid(name);
        public bool ContainsTableOrView(string name) => TablesAndViews.Contains(name);
        #endregion

        #region Routines
        public static void InsertDbData(DbConnection connection, string tableName, DataGrid table)
        {
            var columns = table.Columns;

            var transaction = connection.BeginTransaction();
            foreach (IDictionary<string, object> row in table.Rows.Select(v => (IDictionary<string, object>)v))
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = $"INSERT INTO '{tableName}' ({string.Join(',', columns.Select(c => $"\"{c.Header}\""))}) VALUES ({string.Join(',', columns.Select(c => FormatValue(row[c.Header])))})";

                command.ExecuteNonQuery();
            }
            transaction.Commit();

            static string FormatValue(object value)
            {
                var text = value.ToString();
                if (double.TryParse(text, out _))
                    return text;
                else return $"\"{text}\"";
            }
        }
        #endregion
    }
    #endregion

    #region Utility Functions
    public static class DataTableExtensions
    {
        #region Display
        public static string ToCSV(this DataTable dataTable)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine(string.Join(",", Enumerable.Range(0, dataTable.Columns.Count).Select(i => dataTable.Columns[i].ColumnName.Replace(" ", string.Empty))));
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> items = row.ItemArray.Select(item => $"\"{item.ToString().Replace("\"", "\"\"")}\"");
                string csvLine = string.Join(",", items);
                output.AppendLine(csvLine);
            }
            return output.ToString();
        }
        public static string ToCSVFull(this DataTable dataTable)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dataTable.Columns
                .Cast<System.Data.DataColumn>()
                .Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                {
                    string value = field.ToString();
                    string escapeQuotes = value.Contains('"')
                        ? string.Concat("\"", value.Replace("\"", "\"\""), "\"")
                        : value;
                    string addQuotes = escapeQuotes.Contains(',') ? $"\"{escapeQuotes}\"" : escapeQuotes;
                    return addQuotes;
                });
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }
        public static string Print(this DataTable dataTable)
        {
            ConsoleTable consoleTable = new();
            var columns = Enumerable.Range(0, dataTable.Columns.Count).Select(i => dataTable.Columns[i].ColumnName).ToArray();
            consoleTable.AddColumn(columns);

            foreach (DataRow dr in dataTable.Rows)
            {
                var items = dr.ItemArray.Select(i => i.ToString()).ToArray();
                consoleTable.AddRow(items);
            }
            consoleTable.Write();
            return consoleTable.ToString();
        }
        #endregion

        #region Transformations
        public static DataTable CellSetToTableNew(this CellSet cellset)
        {
            DataTable table = new DataTable("cellset");

            if (cellset.Axes.Count == 2)
            {
                Axis columns = cellset.Axes[0];
                Axis rows = cellset.Axes[1];
                if (rows.Set.Tuples.Count == 0)
                    return null;

                CellCollection valuesCell = cellset.Cells;
                for (int i = 0; i < rows.Set.Hierarchies.Count; i++)
                    table.Columns.Add(CaptureMDXName(rows.Set.Hierarchies[i].UniqueName));
                for (int i = 0; i < columns.Set.Tuples.Count; i++)
                    table.Columns.Add(CaptureMDXName(columns.Set.Tuples[i].Members[0].Caption));

                int rowDimensionCount = rows.Set.Tuples[0].Members.Count;
                int rowCellValuesCount = columns.Set.Tuples.Count;
                int rowElementSize = rowDimensionCount + rowCellValuesCount;
                for (int row = 0; row < rows.Set.Tuples.Count; row++)
                {
                    DataRow tableRow = table.NewRow();

                    for (int i = 0; i < rows.Set.Tuples[row].Members.Count; i++)
                        tableRow[i] = CaptureMDXName(rows.Set.Tuples[row].Members[i].Caption);
                    for (int i = 0; i < columns.Set.Tuples.Count; i++)
                        tableRow[i + rows.Set.Hierarchies.Count] = cellset.Cells[row * rowCellValuesCount + i].Value;

                    table.Rows.Add(tableRow);
                }

                return table;
            }
            else if (cellset.Axes.Count == 1)
            {
                Axis columns = cellset.Axes[0];
                CellCollection valuesCell = cellset.Cells;

                for (int i = 0; i < columns.Set.Tuples.Count; i++)
                    table.Columns.Add(new System.Data.DataColumn(columns.Set.Tuples[i].Members[0].Caption));

                int valuesIndex = 0;
                DataRow row = table.NewRow();
                for (int k = 0; k < columns.Set.Tuples.Count; k++)
                {
                    row[k] = valuesCell[valuesIndex].Value;
                    valuesIndex++;
                }
                table.Rows.Add(row);

                return table;
            }
            else throw new ArgumentException("Unrecognized cellset format.");

            static string CaptureMDXName(string original)
            {
                return Regex.Replace(original, @".*\[(.*?)\]", "$1");
            }
        }
        public static DataTable CellSetToTable(this CellSet cellset)
        {
            DataTable table = new DataTable("cellset");

            Axis columns = cellset.Axes[0];
            Axis rows = cellset.Axes[1];
            CellCollection valuesCell = cellset.Cells;

            table.Columns.Add(rows.Set.Hierarchies[0].Caption);
            for (int i = 0; i < columns.Set.Tuples.Count; i++)
                table.Columns.Add(new System.Data.DataColumn(columns.Set.Tuples[i].Members[0].Caption));
            int valuesIndex = 0;

            for (int i = 0; i < rows.Set.Tuples.Count; i++)
            {
                DataRow row = table.NewRow();

                row[0] = rows.Set.Tuples[i].Members[0].Caption;
                for (int k = 1; k <= columns.Set.Tuples.Count; k++)
                {
                    row[k] = valuesCell[valuesIndex].Value;
                    valuesIndex++;
                }
                table.Rows.Add(row);
            }

            return table;
        }
        #endregion

        #region Queries
        public static DataRow Pick(this DataTable table, Func<DataRow, bool> condition)
            => table.AsEnumerable().Where(condition).First();
        public static IEnumerable<TType> Select<TType>(this DataRowCollection rows, Func<DataRow, TType> action)
        {
            List<TType> result = [];
            foreach (DataRow item in rows)
                result.Add(action(item));
            return result;
        }
        public static bool Empty(this DataTable table)
            => table.Rows.Count == 0;
        #endregion

        #region Object-Oriented Wrapper
        /// <summary>
        /// Retrieve a list of objects for target type using reflection from the data table;
        /// Type must have public properties; Property names are case insensitive
        /// </summary>
        public static GenericStronglyTypedTable<Type> Unwrap<Type>(this DataTable table, Dictionary<string, string> columnRenamming = null) where Type : TableEntryBase, new()
        {
            if (table.Rows.Count == 0) return new GenericStronglyTypedTable<Type>(Array.Empty<Type>());

            // Get column name mapping
            if (columnRenamming == null)
            {
                columnRenamming = new Dictionary<string, string>();
                foreach (string column in Enumerable.Range(0, table.Columns.Count).Select(i => table.Columns[i].ColumnName))
                    columnRenamming.Add(column.Replace(" ", string.Empty), column);   // Case-sensitive; Do notice though it cannot contain spaces
            }

            // Initialize objects from rows
            List<Type> returnValues = new List<Type>();
            foreach (DataRow row in table.Rows)
            {
                // Construct instance of row type
                Type instance = new Type();

                // Initialize properties of the instance
                foreach (PropertyInfo prop in typeof(Type).GetProperties())
                {
                    string propertyName = prop.Name;
                    if (columnRenamming.ContainsKey(propertyName))
                        prop.SetValue(instance, row[columnRenamming[propertyName]] == DBNull.Value
                            ? null
                            : Convert.ChangeType(row[columnRenamming[propertyName]], prop.PropertyType));
                }

                returnValues.Add(instance);
            }

            return new GenericStronglyTypedTable<Type>(returnValues);
        }
        /// <summary>
        /// Get a single value of given type from the single cell of the read result;
        /// Throw exception when invalid
        /// </summary>
        public static TType Single<TType>(this DataTable table) /*where type : IConvertible - allow nullable*/
        {
            // If there is no row and type is not nullable then throw an exception otherwise return null
            bool canBeNull = !typeof(TType).IsValueType
                || Nullable.GetUnderlyingType(typeof(TType)) != null;
            if (table.Rows.Count == 0)
            {
                if (canBeNull)
                    return default;
                else throw new ArgumentException("Table contains no value.");
            }
            return ChangeType<TType>(table.Rows[0][0]);
        }
        /// <summary>
        /// Extract a list of single type of values from a data table
        /// </summary>
        public static List<TType> List<TType>(this DataTable table, int columnIndex = 0) where TType : IConvertible
        {
            // If there is no row then return null
            if (table.Rows.Count == 0) return null;

            // Read all the rows and get the first element of each row
            List<TType> list = new List<TType>();
            foreach (DataRow row in table.Rows)
                list.Add(ChangeType<TType>(row[columnIndex]));

            return list;
        }
        private static T ChangeType<T>(object value)
        {
            // Get type
            Type t = typeof(T);

            // Determin nullability
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // Return null for nullable
                if (value == null)
                    return default;

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }
        #endregion
    }
    #endregion

    #region Object-Oriented Database Table Extension Base
    public abstract class TableEntryBase
    {
    }
    public class GenericStronglyTypedTable<TType> where TType : TableEntryBase
    {
        public GenericStronglyTypedTable(IEnumerable<TType> rows) => Rows = rows.ToList();
        public List<TType> Rows { get; }
    }
    #endregion

    #region Streamlined Workflow
    public enum QuerySourceType
    {
        SQL, // Query content should target SQL database
        MDX, // Query content should target CUBE databse
        CSV // Query content refers to path to CSV file
    }
    /// <remarks>
    /// Notice the design of this generally assumes once created, the tables' contents will not be modified - all subsequent actions are achieved through views
    /// </remarks>
    public class ProceduralInMemoryDB : InMemorySQLIte
    {
        #region References
        private new InMemorySQLIte DB { get; set; }
        private Dictionary<string, string> ViewProceduresReference { get; set; }
        private Dictionary<string, (QuerySourceType Type, string Query)> SourceQueriesReferences { get; set; }
        #endregion

        #region Properties
        private Dictionary<string, DataTable> DataTables = [];
        private bool GenerateDebugCSVs { get; set; } = true;
        private bool PrintDebugOutputs { get; set; } = true;
        private string DSN { get; set; }
        /// <remarks>
        /// Not properly used/implemented at this moment: nowhere to set it.
        /// </remarks>
        private string MDXDataSource { get; set; }
        #endregion

        #region Construction
        public ProceduralInMemoryDB(string dsn, Dictionary<string, string> views, Dictionary<string, (QuerySourceType Type, string Query)> queries, bool generateDebugCSVs = true, bool printDebugOutputs = true)
        {
            DSN = dsn;
            ViewProceduresReference = views;
            SourceQueriesReferences = queries;
            GenerateDebugCSVs = generateDebugCSVs;
            PrintDebugOutputs = printDebugOutputs;
        }
        #endregion

        #region Operators
        public new DataTable this[string name] => DataTables[name];
        #endregion

        #region Public Interface Methods
        public void Prep(string tableName, Dictionary<string, object> parameters = null, string queryName = null)
        {
            if (Tables.Contains(tableName))
                throw new ArgumentException($"Table {tableName} already exists.");

            queryName ??= tableName;
            var queryInfo = SourceQueriesReferences[queryName];
            string query = FormatQuery(SourceQueriesReferences[queryName].Query, parameters);
            Load(tableName, queryInfo.Type, query);
        }
        public void View(string viewName, Dictionary<string, object> parameters = null, string queryName = null)
        {
            if (Tables.Contains(viewName))
                throw new ArgumentException($"View {viewName} already exists.");

            queryName ??= viewName;
            string query = FormatQuery(ViewProceduresReference[queryName], parameters);

            Transform(viewName, query);
        }
        /// <summary>
        /// Reload a table from query list and potentially rewrite existing table.
        /// </summary>
        public void Rewrite(string tableName, Dictionary<string, object> parameters = null, string queryNameOverride = null)
        {
            string queryName = queryNameOverride != null ? queryNameOverride : tableName;

            var queryInfo = SourceQueriesReferences[queryName];
            string query = FormatQuery(SourceQueriesReferences[queryName].Query, parameters);
            Load(tableName, queryInfo.Type, query);
        }
        /// <summary>
        /// Import a dataframe directly as in-memory table; If table already exists, raise an exception. Use Import() directly if you wish to rewrite.
        /// </summary>
        public void Ingest(string tableName, DataTable data)
        {
            if (DataTables.ContainsKey(tableName))
                throw new ArgumentException($"Table {tableName} already exists.");

            DataTables[tableName] = data;
            Import(data, tableName);
            PerformBookkeepingRoutine(data, tableName);
        }
        public void Load(string tableName, QuerySourceType type, string query)
        {
            DB.RunSQL($"DROP TABLE IF EXISTS \"{tableName}\"");
            if (type == QuerySourceType.SQL)
            {
                var odbcConnection = new OdbcConnection($"DSN={DSN}");
                odbcConnection.Open();

                DataTable datatable = new();
                datatable.Load(new OdbcCommand(query, odbcConnection).ExecuteReader());
                DB.Import(datatable, tableName);
                PerformBookkeepingRoutine(datatable, tableName);
            }
            else if (type == QuerySourceType.MDX)
            {
                var datatable = ExecuteMDXQuery(MDXDataSource, query);
                DB.Import(datatable, tableName);
                PerformBookkeepingRoutine(datatable, tableName);
            }
            else if (type == QuerySourceType.CSV)
            {
                string filePath = query;
                if (File.Exists(filePath))
                {
                    DB.Import(CSVHelper.ReadCSVFile(filePath, out string[] headers, true), headers, tableName, out DataTable table);
                    PerformBookkeepingRoutine(table, tableName);
                }
            }
        }
        public void Transform(string viewName, string query)
        {
            DB.RunSQL($"DROP VIEW IF EXISTS \"{viewName}\"");
            DB.RunSQL($"CREATE VIEW \"{viewName}\" AS " + Environment.NewLine + query);

            DataTable datatable = DB.Execute(query);
            PerformBookkeepingRoutine(datatable, viewName);
        }
        public void Export()
            => DB.Export();
        #endregion

        #region Routines
        private void PerformBookkeepingRoutine(DataTable dataTable, string tableName)
        {
            DataTables[tableName] = dataTable;

            Console.WriteLine($"# {tableName}");
            if (PrintDebugOutputs) dataTable.Print();
            if (GenerateDebugCSVs) File.WriteAllText($"{tableName}.csv", dataTable.ToCSV());
        }
        #endregion

        #region Helpers
        private static string FormatQuery(string template, Dictionary<string, object>? parameters = null)
        {
            parameters ??= [];

            if (parameters.Any(p => !p.Key.StartsWith('@')))
                throw new ArgumentException("Parameter name must start with \"@\" symbol.");

            return parameters.Aggregate(template, (current, parameter) => current.Replace(parameter.Key, parameter.Value.ToString()));
        }
        #endregion
    }
    #endregion
}
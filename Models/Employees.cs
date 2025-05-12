using System.Data;
using System.Data.SqlClient;

namespace BattachApp.Models
{
    public class token
    {
        public string Token { get; set; }
    }
    public class geoLocation
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
    public class Payer
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
    }
    public class Students
    {
        public int id { get; set; }
        public string name { get; set; }
        public string school { get; set; }
        public int age { get; set; }
    }
    public class Employees
    {
        public int id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public IFormFile file { get; set; }
        public string image { get; set; }
        public int count { get; set; }
        public int count2 { get; set; }

    }
    public class EmployeesList : IEmployeesList
    {
        public SqlConnection conn;
        public readonly IConfiguration _config;
        public EmployeesList(IConfiguration config)
        {
            _config = config;
        }

        public int GetCount()
        {
            Employees emp = new Employees();
            DataTable tbl2 = new DataTable();
            using (conn = new SqlConnection(_config["ConnectionStrings:Sql"]))
            {
                conn.Open();
                SqlDataAdapter ada2 = new SqlDataAdapter("SELECT * FROM employees", conn);
                ada2.Fill(tbl2);
                return (int)Math.Ceiling((decimal)tbl2.Rows.Count / 3);
            }
        }
        public List<Employees> GetEmployee(int pg)
        {
            List<Employees> emplist = new List<Employees>();
            DataTable tbl = new DataTable();

                try
                {
                    using (conn = new SqlConnection(_config["ConnectionStrings:Sql"]))
                    {
                        conn.Open();
                        /*SqlCommand cmd = new SqlCommand("SELECT * FROM employees",conn);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            Employees emp = new Employees();
                            emp.id = (int)rdr["id"];
                            emp.fname = (string)rdr["fname"];
                            emp.lname = (string)rdr["lname"];
                            emp.email = (string)rdr["email"];
                            emplist.Add(emp);

                        }*/
                        SqlDataAdapter ada = new SqlDataAdapter("SELECT * FROM employees ORDER BY id OFFSET " + (pg * 3 - 3) + " ROWS FETCH NEXT 3 ROWS ONLY", conn); //WHERE id BETWEEN "+num1+" AND "+num2+"", conn);
                        ada.Fill(tbl);
                        /*var query = from employee in tbl.AsEnumerable()
                                    where employee. //.Field<int>("id") >= num1 && employee.Field<int>("id") <= num2
                                    select new {
                                        Id = employee.Field<int>("id"),
                                        Fname = employee.Field<string>("fname"),
                                        Lname = employee.Field<string>("lname"),
                                        Email = employee.Field<string>("email"),
                                        Image = employee.Field<string>("image")
                                    };
                        foreach(var dr in query)
                        {
                            Employees emp = new Employees();
                            emp.id = dr.Id;
                            emp.fname = dr.Fname;
                            emp.lname = dr.Lname;
                            emp.email = dr.Email;
                            emp.image = dr.Image;
                            emplist.Add(emp);
                        }*/

                        for (int i = 0; i < tbl.Rows.Count; i++)
                        {
                            Employees emp = new Employees();
                            emp.id = (int)tbl.Rows[i]["id"];
                            emp.fname = (string)tbl.Rows[i]["fname"];
                            emp.lname = (string)tbl.Rows[i]["lname"];
                            emp.email = (string)tbl.Rows[i]["email"];
                            emp.image = (string)tbl.Rows[i]["image"];
                            emp.count = tbl.Rows.Count;
                            emplist.Add(emp);
                        }
                        
                        return emplist;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
        }
        public string AddEmployee(Employees emp)
        {
            //conn = new SqlConnection(_config != null ? _config["ConnectionStrings:Sql"] : "");
            try
            {
                using (conn = new SqlConnection(_config["ConnectionStrings:Sql"]))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO employees(fname,lname,email,image) VALUES(@fname,@lname,@email,@file)", conn);
                    cmd.Parameters.AddWithValue("@fname", emp.fname);
                    cmd.Parameters.AddWithValue("@lname", emp.lname);
                    cmd.Parameters.AddWithValue("@email", emp.email);
                    if (emp.file != null)
                    {
                        cmd.Parameters.AddWithValue("@file", (string)emp.file.FileName);
                    }
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return "The data has been inserted successfully!";
                }
            }
            catch (Exception ex)
            {
                return (string)ex.Message;
            }
        
        }
    }
    public interface IEmployeesList
    {
        int GetCount();
        List<Employees> GetEmployee(int pg);
        string AddEmployee(Employees emp);
    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
namespace DB1_Project
{
    class Program
    {
        static SqlConnection connectionString = new SqlConnection(@"Data Source=mohammed-pc\sqlexpress;Initial Catalog=FileSystem;Integrated Security=True");

        static DataTable Select(string sql)
        {
            SqlDataAdapter DataAdapter = new SqlDataAdapter(sql, connectionString);
            DataTable dt = new DataTable();
            DataAdapter.Fill(dt);
            return dt;
        }

        static void Query(string sql)
        {
            SqlCommand myCommand = new SqlCommand(sql, connectionString);
            myCommand.Connection.Open();
            myCommand.ExecuteNonQuery();
            myCommand.Connection.Close();
        }

        static Stack<string> path = new Stack<string>();
        static Stack<int> id = new Stack<int>();

        static void createDatabase()
        {
            int count = 0;
            DataTable dt = Select("select name from sysdatabases ");

            foreach (DataRow d in dt.Rows)
            {
                if (Convert.ToString(d["name"]) != "FileSystem")
                {
                    count++;
                }
                if (dt.Rows.Count == count)
                {
                    Query("create database FileSystem");
                }
            }

            Query("use FileSystem");

            Query(" create table Folder (id int ,FolderName varchar(50) ,ParentID int,primary key (id),foreign key (ParentID) references Folder)");
            Query(" create table [File] (id int ,[FileName] varchar(50) ,ParentID int,content varchar(500),primary key (id), foreign key (ParentID) references Folder )");
            Query(" create table program (id int ,ProgramName varchar(50) ,ParentID int, command varchar(200) ,primary key (id),foreign key (ParentID) references Folder)");
            
            Query(" insert into Folder (FolderName, ParentID) values ('c:', null)");
            Query(" insert into Folder (FolderName, ParentID) values ('windows', 1)");
            Query(" insert into Folder (FolderName, ParentID) values ('program', 1)");
            Query(" insert into Folder (FolderName, ParentID) values ('users', 1)");

            Query(" insert into [File] ([FileName], ParentID, content) values ('mhd.txt',4 , 'hello world')");

            Query(" insert into program (ProgramName, ParentID) values ('calc.exe', 3)");
            Query(" insert into program (ProgramName, ParentID) values ('regegit.exe', 3)");

        }

        static void Main(string[] args)
        {
            string root = "c:";
            string instruct = "";
            path.Push(root);
            DataTable dd = Select("select id from Folder where FolderName ='c:'");
            id.Push(Convert.ToInt32(dd.Rows[0]["id"]));

            Console.Write(path.First() + ">");
            instruct = Console.ReadLine();

            string[] part = instruct.Split(' ');

            while (instruct != "exit")
            {
                part = instruct.Split(' ');

                if (part[0] == "cd")
                {
                    if (part.Length > 1)
                    {
                        DataTable dt = Select("select * from Folder f join Folder ff on f.id = ff.ParentID where f.FolderName = '" + path.First() + "' and ff.FolderName = '" + part[1] + "'");
                        if (dt.Rows.Count != 0)
                        {
                            path.Push(part[1]);
                            DataTable d = Select("select id from Folder where FolderName='" + path.First() + "' and ParentId=" + id.First() + "");
                            id.Push(Convert.ToInt32(d.Rows[0]["id"]));
                        }
                        else
                        {
                            Console.WriteLine("The system cannot find the path specified!");
                        }
                    }
                }

                else if (part[0] == "cd..")
                {
                    if (path.First() != root)
                    {
                        path.Pop();
                        id.Pop();
                    }
                }

                else if (part[0] == "dir")
                {
                    DataTable dt1 = Select("select * from Folder where ParentID = " + id.First());
                    foreach (DataRow dr in dt1.Rows)
                        Console.WriteLine("              <dir>       " + dr["FolderName"]);

                    DataTable dt3 = Select("select * from program where ParentID = " + id.First());
                    foreach (DataRow dr in dt3.Rows)
                        Console.WriteLine("                          " + dr["ProgramName"]);

                    DataTable dt2 = Select("select * from [File] where ParentID = " + id.First());
                    foreach (DataRow dr in dt2.Rows)
                        Console.WriteLine("                      " + dr["SizeInByte"] + "  " + dr["FileName"]);

                    Console.WriteLine("             " + dt2.Rows.Count + "File<s>          ");
                    Console.WriteLine("             " + dt3.Rows.Count + " program<s>        ");
                    Console.WriteLine("             " + dt1.Rows.Count + " Dir<s>        ");

                }

                else if (part[0] == "mkdir")
                {
                    if (part.Length > 1)
                    {
                        DataTable dt = Select("select * from Folder f join Folder ff on f.id = ff.ParentID where f.FolderName = '" + path.First() + "' and ff.FolderName = '" + part[1] + "'");
                        if (dt.Rows.Count != 0)
                        {
                            Console.WriteLine("A subdirectory or File " + part[1] + " already exist!.");
                        }
                        else
                            Query("insert into Folder (FolderName, ParentID) values ('" + part[1] + "'," + id.First() + ")");
                    }
                }

                else if (part[0] == "rmdir")
                {
                    DataTable dt = Select("select * from Folder f join Folder ff on f.id = ff.ParentID where f.FolderName =  '" + part[1] + "' and f.ParentID=" + id.First() + "");
                    if (dt.Rows.Count != 0)
                        Console.WriteLine("The directory is not empty!");
                    else
                        Query("delete from Folder where FolderName = '" + part[1] + "'");
                }

                else if (part[0] == "rename")
                {
                    if (part.Length > 2)
                    {
                        DataTable dt = Select("select FolderName from Folder where FolderName='" + part[1] + "'");
                        if (dt.Rows.Count != 0)
                        {
                            Query("update Folder set FolderName='" + part[2] + "' where FolderName='" + part[1] + "'");
                        }
                        else
                        {
                            dt = Select("select [FileName] from [File] where [FileName]='" + part[1] + "'");
                            if (dt.Rows.Count != 0)
                            {
                                Query("update [File] set [FileNAme]='" + part[2] + "' where [FileName]='" + part[1] + "'");
                            }
                            else
                                Console.WriteLine("The systen cannot find the path specified!");
                        }
                    }
                }

                else if (part[0] == "mkfile")
                {
                    if (part.Length > 2)
                    {
                        Query("insert into [File] (FileName, Content, ParentID, SizeInByte) values ('" + part[1] + "','" + part[2] + "'," + id.First() + ", " + part[2].Length + ")");
                    }
                }

                else if (part[0] == "rmfile")
                {
                    if (part.Length > 1)
                    {
                        DataTable dt = Select("select * from [File] where [FileName] =  '" + part[1] + "' and ParentId=" + id.First() + " ");

                        if (dt.Rows.Count != 0)
                            Query("delete from [File] where FileName = '" + part[1] + "'");

                        else
                            Console.WriteLine("The directory is not empty!");
                    }
                }

                else if (part[0] == "mkprog")
                {
                    if (part.Length > 2)
                    {
                        Query("insert into Program (ProgramName,Command, ParentID) values ('" + part[1] + "','" + part[2] + "',(select id from Folder where FolderName = '" + path.First() + "'))");
                    }
                }

                else if (part[0] == "rmprog")
                {
                    if (part.Length > 1)
                    {
                        DataTable dt = Select("select * from Folder f join Program ff on f.id = ff.ParentID where ff.ProgramName =  '" + part[1] + "'");
                        if (dt.Rows.Count != 0)
                            Query("delete from Program where ProgramName = '" + part[1] + "'");
                        else
                            Console.WriteLine("The directory is not empty!");
                    }
                }

                else if (part[0] == "open")
                {
                    if (part.Length > 1)
                    {
                        DataTable dt = Select("select Content from [File] where ParentID=(select id from Folder where FolderName='" + path.First() + "') and [FileName]='" + part[1] + "'");
                        if (dt.Rows.Count != 0)
                        {
                            Console.WriteLine(dt.Rows[0]["content"]);
                        }
                        else
                            Console.WriteLine("the File doesn't exist!.");
                    }
                }

                else if (part[0] == "exec")
                {
                    if (part.Length > 1)
                    {
                        int x = part.Length;
                        DataTable dt = Select("select Command from Program where ParentID=(select id from Folder where FolderName='" + path.First() + "') and ProgramName='" + part[1] + "'");
                        if (dt.Rows.Count != 0)
                        {
                            Console.Write(dt.Rows[0]["command"]);
                            for (int i = 2; i < x; i++)
                            {
                                Console.Write(" {0} ", part[i]);                                      
                            }
                            Console.WriteLine();
                        }
                        else
                            Console.WriteLine("The program doesn't exist!.");
                    }
                }

                else if (part[0] == "search")
                {
                    if (part.Length > 1)
                    {
                        Stack<string> t = new Stack<string>();
                        DataTable dt = Select("select * from [File] where FileName = '" + part[1] + "'");
                        if (dt.Rows.Count != 0)
                        {
                            t.Push(Convert.ToString(dt.Rows[0]["FileName"]));
                            int x = Convert.ToInt32(dt.Rows[0]["ParentID"]);
                            dt = Select("select * from [Folder] where ParentID is not null and id = '" + x.ToString() + "'");
                            while (dt.Rows.Count != 0)
                            {
                                x = Convert.ToInt32(dt.Rows[0]["ParentId"]);
                                t.Push(Convert.ToString(dt.Rows[0]["FolderName"]));
                                dt = Select("select * from [Folder] where ParentID is not null and id = '" + x.ToString() + "'");
                            }

                            dt = Select("select * from [Folder] where id = '" + x.ToString() + "'");
                            t.Push(Convert.ToString(dt.Rows[0]["FolderName"]));
                            Console.WriteLine(string.Join("\\", t));
                        }
                        else
                        {
                            Console.WriteLine("The file is not existing!");
                        }
                    }
                }

                Console.Write(string.Join("\\", path.Reverse()) + ">");
                instruct = Console.ReadLine();
            }
        }
    }
}

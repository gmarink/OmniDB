﻿/*
Copyright 2016 The OmniDB Team

This file is part of OmniDB.

OmniDB is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

OmniDB is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with OmniDB. If not, see http://www.gnu.org/licenses/.
*/

using System;
using System.Web;
using System.Web.UI;

namespace OmniDB
{
	/// <summary>
	/// Tree initial information.
	/// </summary>
	public class TreeReturn {
		public string v_mode;
		public TreeDatabaseReturn v_database_return;
	}

	/// <summary>
	/// Tree database initial information.
	/// </summary>
	public class TreeDatabaseReturn {
		public string v_database;
		public string v_schema;
		public bool v_has_schema;
	}

	/// <summary>
	/// Contains all webmethods to retrieve database components to render the treeview.
	/// </summary>
	public partial class Tree : System.Web.UI.Page
	{

		/// <summary>
		/// Get database information to render initial tree of components.
		/// </summary>
		[System.Web.Services.WebMethod]
		public static AjaxReturn GetTreeInfo()
		{
			AjaxReturn v_return = new AjaxReturn();
			TreeReturn v_tree_data = new TreeReturn ();
			Session v_session = (Session)System.Web.HttpContext.Current.Session ["OMNIDB_SESSION"];

			if (v_session == null) 
			{
				v_return.v_error = true;
				v_return.v_error_id = 1;
				return v_return;
			} 


			v_tree_data.v_mode = "database";

			TreeDatabaseReturn v_database_return = new TreeDatabaseReturn ();

			OmniDatabase.Generic v_database = v_session.GetSelectedDatabase ();

			v_database_return.v_database = v_database.GetName ();
			v_database_return.v_has_schema = v_database.v_has_schema;

			if (v_database_return.v_has_schema)
				v_database_return.v_schema = v_database.v_schema;

			v_tree_data.v_database_return = v_database_return;

			v_return.v_data = v_tree_data;

			return v_return;



		}

		/// <summary>
		/// Get all database tables.
		/// </summary>
		[System.Web.Services.WebMethod]
		public static AjaxReturn GetTables()
		{
			AjaxReturn v_return = new AjaxReturn();
			Session v_session = (Session)System.Web.HttpContext.Current.Session ["OMNIDB_SESSION"];

			if (v_session == null) 
			{
				v_return.v_error = true;
				v_return.v_error_id = 1;
				return v_return;
			} 

			OmniDatabase.Generic v_database = v_session.GetSelectedDatabase();

			System.Collections.Generic.List<string> v_list_tables = new System.Collections.Generic.List<string> ();

			try
			{
				System.Data.DataTable v_tables = v_database.QueryTables(false);

				foreach (System.Data.DataRow v_table in v_tables.Rows)
					v_list_tables.Add(v_table["table_name"].ToString());

			}
			catch (Spartacus.Database.Exception e)
			{

				v_return.v_error = true;
				v_return.v_data = e.v_message.Replace("<","&lt;").Replace(">","&gt;").Replace(System.Environment.NewLine, "<br/>");

				return v_return;
			}


			v_return.v_data = v_list_tables;

			return v_return;

		}

		/// <summary>
		/// Get all database views.
		/// </summary>
		[System.Web.Services.WebMethod]
		public static AjaxReturn GetViews()
		{
			AjaxReturn v_return = new AjaxReturn();
			Session v_session = (Session)System.Web.HttpContext.Current.Session ["OMNIDB_SESSION"];

			if (v_session == null) 
			{
				v_return.v_error = true;
				v_return.v_error_id = 1;
				return v_return;
			} 

			OmniDatabase.Generic v_database = v_session.GetSelectedDatabase();

			System.Collections.Generic.List<string> v_list_views = new System.Collections.Generic.List<string> ();

			try
			{
				System.Data.DataTable v_views = v_database.QueryViews();

				foreach (System.Data.DataRow v_table in v_views.Rows)
					v_list_views.Add(v_table["view_name"].ToString());

			}
			catch (Spartacus.Database.Exception e)
			{

				v_return.v_error = true;
				v_return.v_data = e.v_message.Replace("<","&lt;").Replace(">","&gt;").Replace(System.Environment.NewLine, "<br/>");

				return v_return;
			}


			v_return.v_data = v_list_views;

			return v_return;

		}

		/// <summary>
		/// Get columns of a database table.
		/// </summary>
		/// <param name="p_table">Table name.</param>
		[System.Web.Services.WebMethod]
		public static AjaxReturn GetColumns(string p_table)
		{
			AjaxReturn v_return = new AjaxReturn();
			Session v_session = (Session)System.Web.HttpContext.Current.Session ["OMNIDB_SESSION"];

			if (v_session == null) 
			{
				v_return.v_error = true;
				v_return.v_error_id = 1;
				return v_return;
			} 

			OmniDatabase.Generic v_database = v_session.GetSelectedDatabase();

			System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list_columns = new System.Collections.Generic.List<System.Collections.Generic.List<string>> ();

			try
			{

				System.Data.DataTable v_columns = v_database.QueryTablesFields(p_table);

				foreach (System.Data.DataRow v_column in v_columns.Rows)
				{

					System.Collections.Generic.List<string> v_column_details = new System.Collections.Generic.List<string>();

					v_column_details.Add(v_column["column_name"].ToString());
					v_column_details.Add(v_column["data_type"].ToString());
					v_column_details.Add(v_column["data_length"].ToString());
					v_column_details.Add(v_column["nullable"].ToString());

					v_list_columns.Add(v_column_details);
				}

			}
			catch (Spartacus.Database.Exception e)
			{

				v_return.v_error = true;
				v_return.v_data = e.v_message.Replace("<","&lt;").Replace(">","&gt;").Replace(System.Environment.NewLine, "<br/>");

				return v_return;
			}

			v_return.v_data = v_list_columns;

			return v_return;

		}

		/// <summary>
		/// Get unique constraints of a database table.
		/// </summary>
		/// <param name="p_table">Table name.</param>
		[System.Web.Services.WebMethod]
		public static AjaxReturn GetUniques(string p_table)
		{
			AjaxReturn v_return = new AjaxReturn();
			Session v_session = (Session)System.Web.HttpContext.Current.Session ["OMNIDB_SESSION"];

			if (v_session == null) 
			{
				v_return.v_error = true;
				v_return.v_error_id = 1;
				return v_return;
			} 

			OmniDatabase.Generic v_database = v_session.GetSelectedDatabase();

			System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list_pks = new System.Collections.Generic.List<System.Collections.Generic.List<string>> ();

			try
			{
				System.Data.DataTable v_table_fks = v_database.QueryTablesUniques("",p_table);

				foreach (System.Data.DataRow v_table_pk in v_table_fks.Rows)
				{
					System.Collections.Generic.List<string> v_fk = new System.Collections.Generic.List<string>();

					v_fk.Add(v_table_pk["constraint_name"].ToString());
					v_fk.Add(v_table_pk["column_name"].ToString());

					v_list_pks.Add(v_fk);
				}
			}
			catch (Spartacus.Database.Exception e)
			{

				v_return.v_error = true;
				v_return.v_data = e.v_message.Replace("<","&lt;").Replace(">","&gt;").Replace(System.Environment.NewLine, "<br/>");

				return v_return;
			}


			v_return.v_data = v_list_pks;

			return v_return;

		}

		/// <summary>
		/// Get indexes of a database table.
		/// </summary>
		/// <param name="p_table">Table name.</param>
		[System.Web.Services.WebMethod]
		public static AjaxReturn GetIndexes(string p_table)
		{
			AjaxReturn v_return = new AjaxReturn();
			Session v_session = (Session)System.Web.HttpContext.Current.Session ["OMNIDB_SESSION"];

			if (v_session == null) 
			{
				v_return.v_error = true;
				v_return.v_error_id = 1;
				return v_return;
			} 

			OmniDatabase.Generic v_database = v_session.GetSelectedDatabase();

			System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list_pks = new System.Collections.Generic.List<System.Collections.Generic.List<string>> ();

			try
			{
				System.Data.DataTable v_table_fks = v_database.QueryTablesIndexes(p_table);

				foreach (System.Data.DataRow v_table_pk in v_table_fks.Rows)
				{

					System.Collections.Generic.List<string> v_fk = new System.Collections.Generic.List<string>();

					v_fk.Add(v_table_pk["index_name"].ToString());
					v_fk.Add(v_table_pk["uniqueness"].ToString());
					v_fk.Add(v_table_pk["column_name"].ToString());

					v_list_pks.Add(v_fk);
				}
			}
			catch (Spartacus.Database.Exception e)
			{

				v_return.v_error = true;
				v_return.v_data = e.v_message.Replace("<","&lt;").Replace(">","&gt;").Replace(System.Environment.NewLine, "<br/>");

				return v_return;
			}


			v_return.v_data = v_list_pks;

			return v_return;

		}

		/// <summary>
		/// Get primary key of a database table.
		/// </summary>
		/// <param name="p_table">Table name.</param>
		[System.Web.Services.WebMethod]
		public static AjaxReturn GetPK(string p_table)
		{
			AjaxReturn v_return = new AjaxReturn();
			Session v_session = (Session)System.Web.HttpContext.Current.Session ["OMNIDB_SESSION"];

			if (v_session == null) 
			{
				v_return.v_error = true;
				v_return.v_error_id = 1;
				return v_return;
			} 

			OmniDatabase.Generic v_database = v_session.GetSelectedDatabase();

			System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list_pks = new System.Collections.Generic.List<System.Collections.Generic.List<string>> ();

			try
			{
				System.Data.DataTable v_table_fks = v_database.QueryTablesPrimaryKeys("", p_table);

				foreach (System.Data.DataRow v_table_pk in v_table_fks.Rows)
				{
					System.Collections.Generic.List<string> v_fk = new System.Collections.Generic.List<string>();

					v_fk.Add(v_table_pk["constraint_name"].ToString());
					v_fk.Add(v_table_pk["column_name"].ToString());

					v_list_pks.Add(v_fk);
				}
			}
			catch (Spartacus.Database.Exception e)
			{

				v_return.v_error = true;
				v_return.v_data = e.v_message.Replace("<","&lt;").Replace(">","&gt;").Replace(System.Environment.NewLine, "<br/>");

				return v_return;
			}


			v_return.v_data = v_list_pks;

			return v_return;

		}


		/// <summary>
		/// Get foreign keys of a database table.
		/// </summary>
		/// <param name="p_table">Table name.</param>
		[System.Web.Services.WebMethod]
		public static AjaxReturn GetFKs(string p_table)
		{
			AjaxReturn v_return = new AjaxReturn();
			Session v_session = (Session)System.Web.HttpContext.Current.Session ["OMNIDB_SESSION"];

			if (v_session == null) 
			{
				v_return.v_error = true;
				v_return.v_error_id = 1;
				return v_return;
			} 

			OmniDatabase.Generic v_database = v_session.GetSelectedDatabase();

			System.Collections.Generic.List<System.Collections.Generic.List<string>> v_list_fks = new System.Collections.Generic.List<System.Collections.Generic.List<string>> ();

			try
			{
				System.Data.DataTable v_table_fks = v_database.QueryTablesForeignKeys(p_table);

				foreach (System.Data.DataRow v_table_fk in v_table_fks.Rows)
				{
					System.Collections.Generic.List<string> v_fk = new System.Collections.Generic.List<string>();

					v_fk.Add(v_table_fk["constraint_name"].ToString());
					v_fk.Add(v_table_fk["column_name"].ToString());
					v_fk.Add(v_table_fk["r_table_name"].ToString());
					v_fk.Add(v_table_fk["r_column_name"].ToString());
					v_fk.Add(v_table_fk["delete_rule"].ToString());
					v_fk.Add(v_table_fk["update_rule"].ToString());

					v_list_fks.Add(v_fk);
				}
			}
			catch (Spartacus.Database.Exception e)
			{

				v_return.v_error = true;
				v_return.v_data = e.v_message.Replace("<","&lt;").Replace(">","&gt;").Replace(System.Environment.NewLine, "<br/>");

				return v_return;
			}


			v_return.v_data = v_list_fks;

			return v_return;

		}
	}

}


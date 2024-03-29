﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Extensions
{
    public static partial class DapperExtension
    {
        #region common method for ado.net

        public static async Task<DataTable> GetDataTableAsync(this IDbConnection conn, string sql, object param = null, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await Task.Run(() =>
            {
                return GetDataTable(conn, sql, param, tran, commandTimeout, commandType);
            });
        }

        public static async Task<DataSet> GetDataSetAsync(this IDbConnection conn, string sql, object param = null, IDbTransaction tran = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await Task.Run(() =>
            {
                return GetDataSet(conn, sql, param, tran, commandTimeout, commandType);
            });
        }

        public static async Task<DataTable> GetSchemaTableAsync<T>(this IDbConnection conn, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            return await Task.Run(() =>
            {
                return GetSchemaTable<T>(conn, returnFields, tran, commandTimeout);
            });
        }

        public static async Task<string> BulkCopyAsync(this IDbConnection conn, DataTable dt, string tableName, string copyFields = null, bool insert_identity = false, int batchSize = 20000, int timeout = 100)
        {
            return await Task.Run(() =>
            {
                return BulkCopy(conn, dt, tableName, copyFields, insert_identity, batchSize, timeout);
            });

        }

        public static async Task<string> BulkCopyAsync<T>(this IDbConnection conn, DataTable dt, string copyFields = null, bool insert_identity = false, int batchSize = 20000, int timeout = 100)
        {
            return await Task.Run(() =>
            {
                return BulkCopy<T>(conn, dt, copyFields, insert_identity, batchSize, timeout);
            });
        }

        public static async Task<string> BulkUpdateAsync(this IDbConnection conn, DataTable dt, string tableName, string column = "*", int batchSize = 20000, int timeout = 100)
        {
            return await Task.Run(() =>
            {
                return BulkUpdate(conn, dt, tableName, column, batchSize, timeout);
            });
        }

        public static async Task<string> BulkUpdateAsync<T>(this IDbConnection conn, DataTable dt, string column = "*", int batchSize = 20000, int timeout = 100)
        {
            return await Task.Run(() =>
            {
                return BulkUpdate<T>(conn, dt, column, batchSize, timeout);
            });
        }

        #endregion

        #region method (Insert Update Delete)

        public static async Task<int> InsertAsync<T>(this IDbConnection conn, T model, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteAsync(builder.GetInsertSql<T>(), model, tran, commandTimeout);
        }

        public static async Task<dynamic> InsertReturnIdAsync<T>(this IDbConnection conn, T model, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteScalarAsync<dynamic>(builder.GetInsertReturnIdSql<T>(), model, tran, commandTimeout);
        }

        public async static Task<decimal> InsertReturnIdForOracleAsync<T>(this IDbConnection conn, T model, string sequence, IDbTransaction tran = null, int? commandTimeout = null)
        {
            return await Task.Run(() =>
            {
                var builder = BuilderFactory.GetBuilder(conn);
                conn.Execute(builder.GetInsertReturnIdSql<T>(sequence), model, tran, commandTimeout);
                return GetSequenceCurrent<decimal>(conn, sequence, tran, null);
            });

        }

        public static async Task<int> InsertIdentityAsync<T>(this IDbConnection conn, T model, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteAsync(builder.GetInsertIdentitySql<T>(), model, tran, commandTimeout);
        }

        public static async Task<int> UpdateAsync<T>(this IDbConnection conn, T model, string updateFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteAsync(builder.GetUpdateSql<T>(updateFields), model, tran, commandTimeout);
        }

        public static async Task<int> UpdateByWhereAsync<T>(this IDbConnection conn, T model, string where, string updateFields, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteAsync(builder.GetUpdateByWhereSql<T>(where, updateFields), model, tran, commandTimeout);
        }

        public static async Task<int> InsertOrUpdateAsync<T>(this IDbConnection conn, T model, string updateFields = null, bool update = true, IDbTransaction tran = null, int? commandTimeout = null)
        {
            return await Task.Run(() =>
            {
                return InsertOrUpdate<T>(conn, model, updateFields, update, tran, commandTimeout);
            });
        }

        public static async Task<int> InsertIdentityOrUpdateAsync<T>(this IDbConnection conn, T model, string updateFields = null, bool update = true, IDbTransaction tran = null, int? commandTimeout = null)
        {
            return await Task.Run(() =>
            {
                return InsertIdentityOrUpdate<T>(conn, model, updateFields, update, tran, commandTimeout);
            });
        }

        public static async Task<int> DeleteAsync<T>(this IDbConnection conn, object id, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteAsync(builder.GetDeleteByIdSql<T>(), new { id = id }, tran, commandTimeout);
        }

        public static async Task<int> DeleteByIdsAsync<T>(this IDbConnection conn, object ids, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return 0;
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return await conn.ExecuteAsync(builder.GetDeleteByIdsSql<T>(), dpar, tran, commandTimeout);
        }

        public static async Task<int> DeleteByWhereAsync<T>(this IDbConnection conn, string where, object param, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteAsync(builder.GetDeleteByWhereSql<T>(where), param, tran, commandTimeout);
        }

        public static async Task<int> DeleteAllAsync<T>(this IDbConnection conn, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteAsync(builder.GetDeleteAllSql<T>(), null, tran, commandTimeout);

        }


        #endregion

        #region method (Query)

        public static async Task<IdType> GetIdentityAsync<IdType>(this IDbConnection conn, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteScalarAsync<IdType>(builder.GetIdentitySql(), null, tran, commandTimeout);
        }

        public static async Task<IdType> GetSequenceCurrentAsync<IdType>(this IDbConnection conn, string sequence, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteScalarAsync<IdType>(builder.GetSequenceCurrentSql(sequence), null, tran, commandTimeout);
        }

        public static async Task<IdType> GetSequenceNextAsync<IdType>(this IDbConnection conn, string sequence, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteScalarAsync<IdType>(builder.GetSequenceNextSql(sequence), null, tran, commandTimeout);
        }

        public static async Task<long> GetTotalAsync<T>(this IDbConnection conn, string where = null, object param = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.ExecuteScalarAsync<long>(builder.GetTotalSql<T>(where), param, tran, commandTimeout);
        }

        public static async Task<IEnumerable<T>> GetAllAsync<T>(this IDbConnection conn, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryAsync<T>(builder.GetAllSql<T>(returnFields, orderBy), null, tran, commandTimeout);
        }

        public static async Task<IEnumerable<dynamic>> GetAllDynamicAsync<T>(this IDbConnection conn, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryAsync(builder.GetAllSql<T>(returnFields, orderBy), null, tran, commandTimeout);
        }

        public static async Task<T> GetByIdAsync<T>(this IDbConnection conn, object id, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryFirstOrDefaultAsync<T>(builder.GetByIdSql<T>(returnFields), new { id = id }, tran, commandTimeout);
        }

        public static async Task<dynamic> GetByIdDynamicAsync<T>(this IDbConnection conn, object id, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryFirstOrDefaultAsync<dynamic>(builder.GetByIdSql<T>(returnFields), new { id = id }, tran, commandTimeout);
        }

        public static async Task<IEnumerable<T>> GetByIdsAsync<T>(this IDbConnection conn, object ids, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return Enumerable.Empty<T>();
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return await conn.QueryAsync<T>(builder.GetByIdsSql<T>(returnFields), dpar, tran, commandTimeout);
        }

        public static async Task<IEnumerable<dynamic>> GetByIdsDynamicAsync<T>(this IDbConnection conn, object ids, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return Enumerable.Empty<dynamic>();
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return await conn.QueryAsync(builder.GetByIdsSql<T>(returnFields), dpar, tran, commandTimeout);
        }

        public static async Task<IEnumerable<T>> GetByIdsWithFieldAsync<T>(this IDbConnection conn, object ids, string field, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return Enumerable.Empty<T>();
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return await conn.QueryAsync<T>(builder.GetByIdsWithFieldSql<T>(field, returnFields), dpar, tran, commandTimeout);
        }

        public static async Task<IEnumerable<dynamic>> GetByIdsWithFieldDynamicAsync<T>(this IDbConnection conn, object ids, string field, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            if (CommonUtil.ObjectIsEmpty(ids))
                return Enumerable.Empty<dynamic>();
            var builder = BuilderFactory.GetBuilder(conn);
            DynamicParameters dpar = new DynamicParameters();
            dpar.Add("@ids", ids);
            return await conn.QueryAsync(builder.GetByIdsWithFieldSql<T>(field, returnFields), dpar, tran, commandTimeout);
        }

        public static async Task<IEnumerable<T>> GetByWhereAsync<T>(this IDbConnection conn, string where, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryAsync<T>(builder.GetByWhereSql<T>(where, returnFields, orderBy), param, tran, commandTimeout);
        }

        public static async Task<IEnumerable<dynamic>> GetByWhereDynamicAsync<T>(this IDbConnection conn, string where, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryAsync(builder.GetByWhereSql<T>(where, returnFields, orderBy), param, tran, commandTimeout);
        }

        public static async Task<T> GetByWhereFirstAsync<T>(this IDbConnection conn, string where, object param = null, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryFirstOrDefaultAsync<T>(builder.GetByWhereFirstSql<T>(where, returnFields), param, tran, commandTimeout);
        }

        public static async Task<dynamic> GetByWhereFirstDynamicAsync<T>(this IDbConnection conn, string where, object param = null, string returnFields = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryFirstOrDefaultAsync<dynamic>(builder.GetByWhereFirstSql<T>(where, returnFields), param, tran, commandTimeout);
        }

        public static async Task<IEnumerable<T>> GetBySkipTakeAsync<T>(this IDbConnection conn, int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryAsync<T>(builder.GetBySkipTakeSql<T>(skip, take, where, returnFields, orderBy), param, tran, commandTimeout);
        }

        public static async Task<IEnumerable<dynamic>> GetBySkipTakeDynamicAsync<T>(this IDbConnection conn, int skip, int take, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryAsync(builder.GetBySkipTakeSql<T>(skip, take, where, returnFields, orderBy), param, tran, commandTimeout);
        }

        public static async Task<IEnumerable<T>> GetByPageIndexAsync<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryAsync<T>(builder.GetByPageIndexSql<T>(pageIndex, pageSize, where, returnFields, orderBy), param, tran, commandTimeout);
        }

        public static async Task<IEnumerable<dynamic>> GetByPageIndexDynamicAsync<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            var builder = BuilderFactory.GetBuilder(conn);
            return await conn.QueryAsync(builder.GetByPageIndexSql<T>(pageIndex, pageSize, where, returnFields, orderBy), param, tran, commandTimeout);
        }

        public static async Task<PageEntity<T>> GetPageAsync<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            return await Task.Run(() =>
            {
                return GetPage<T>(conn, pageIndex, pageSize, where, param, returnFields, orderBy, tran, commandTimeout);
            });
        }

        public static async Task<PageEntity<dynamic>> GetPageDynamicAsync<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            return await Task.Run(() =>
            {
                return GetPageDynamic<T>(conn, pageIndex, pageSize, where, param, returnFields, orderBy, tran, commandTimeout);
            });
        }

        public static async Task<PageEntity<T>> GetPageForOracleAsync<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            return await Task.Run(() =>
            {
                return GetPageForOracle<T>(conn, pageIndex, pageSize, where, param, returnFields, orderBy, tran, commandTimeout);
            });
        }

        public static async Task<PageEntity<dynamic>> GetPageDynamicOracleAsync<T>(this IDbConnection conn, int pageIndex, int pageSize, string where = null, object param = null, string returnFields = null, string orderBy = null, IDbTransaction tran = null, int? commandTimeout = null)
        {
            return await Task.Run(() =>
            {
                return GetPageForOracleDynamic<T>(conn, pageIndex, pageSize, where, param, returnFields, orderBy, tran, commandTimeout);
            });
        }

        #endregion

    }
}

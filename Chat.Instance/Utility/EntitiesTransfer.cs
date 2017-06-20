using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Chat.Instance.Utility
{
    public static class EntitiesTransfer
    {
        /// <summary>
        /// datatable to list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> list = new List<T>();
            if (dt == null) return list;
            if (dt.Rows.Count < 1) return list;
            DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilder(dt.Rows[0]);
            foreach (DataRow row in dt.Rows)
                list.Add(eblist.Build(row));
            return list;
        }

        /// <summary>
        /// datatable to list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt, out int totalRecords)
        {
            totalRecords = 0;
            List<T> list = new List<T>();
            if (dt == null || dt.Rows.Count < 1) return list;
    
            DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilder(dt.Rows[0]);
            foreach (DataRow row in dt.Rows)
                list.Add(eblist.Build(row));
            totalRecords = list.Count;
            return list;
        }

        /// <summary>
        /// datatable to list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> PageList<T>(this DataTable dt, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            List<T> list = new List<T>();
            if (dt == null || dt.Rows.Count < 1) return list;

            DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilder(dt.Rows[0]);
            foreach (DataRow row in dt.Rows)
                list.Add(eblist.Build(row));
            totalRecords = list.Count;
            list = list.PageList<T>(pageIndex, pageSize);
            return list;
        }

        public static T ToEntity<T>(this DataRow row)
        {
            T entity = default(T);

            if (null != row)
            {
                DataTableEntityBuilder<T> eblist = DataTableEntityBuilder<T>.CreateBuilder(row);
                entity = eblist.Build(row);
            }

            return entity;
        }

        public static List<T> PageList<T>(this List<T> entities, int pageIndex, int pageSize)
        {
            if (null == entities)
            {
                return default(List<T>);
            }

            int index = (pageIndex - 1) * pageSize;
            int count = entities.Count - index > pageSize ? pageSize : entities.Count - index;
            return entities.GetRange(index, count);
        }
    }

    /// <summary>
    /// builder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataTableEntityBuilder<T>
    {
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
        private delegate T Load(DataRow dataRecord);
        private Load handler;
        private DataTableEntityBuilder() { }

        public T Build(DataRow row)
        {
            return handler(row);
        }

        public static DataTableEntityBuilder<T> CreateBuilder(DataRow dataRow)
        {
            DataTableEntityBuilder<T> dynamicBuilder = new DataTableEntityBuilder<T>();
            DynamicMethod method = new DynamicMethod("DynamicCreateEntity", typeof(T), new Type[] { typeof(DataRow) }, typeof(T), true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int index = 0; index < dataRow.ItemArray.Length; index++)
            {
                PropertyInfo propertyInfo = typeof(T).GetProperty(dataRow.Table.Columns[index].ColumnName);
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, index);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, index);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }
    }    
}

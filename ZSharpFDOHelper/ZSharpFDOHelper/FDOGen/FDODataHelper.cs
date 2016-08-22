using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;

namespace ZSharpFDOHelper.FDOGen
{
    public class FDODataHelper
    {

        public static Expression ParseByDataType(string data, DataType dataType)
        {
            Expression expr = null;
            bool bIsNull = false;

            // NOTE: blob parsing doesn't work yet (ever?) in FDO:
            if (dataType != DataType.DataType_BLOB)
            {
                expr = Expression.Parse(data);

                if (expr is BooleanValue)
                {
                    bIsNull = true;
                }
                else
                {
                    bIsNull = false;
                }
            }

            switch (dataType)
            {
                case DataType.DataType_Boolean:
                    {
                        if (bIsNull)
                        {
                            BooleanValue val = new BooleanValue();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            BooleanValue value = (BooleanValue)(expr);
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                        }
                    }
                    break;

                case DataType.DataType_Byte:
                    {
                        if (bIsNull)
                        {
                            ByteValue val = new ByteValue();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            Int32Value value = (Int32Value)(expr);
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                            expr = new ByteValue((byte)value.Int32);
                        }
                    }
                    break;

                case DataType.DataType_Int16:
                    {
                        if (bIsNull)
                        {
                            Int16Value val = new Int16Value();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            Int32Value value = (Int32Value)(expr);
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                            expr = new Int16Value((Int16)value.Int32);
                        }
                    }
                    break;

                case DataType.DataType_Int32:
                    {
                        if (bIsNull)
                        {
                            Int32Value val = new Int32Value();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            Int32Value value = (Int32Value)(expr);
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                        }
                    }
                    break;

                case DataType.DataType_Int64:
                    {
                        if (bIsNull)
                        {
                            Int64Value val = new Int64Value();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            Int64Value value = (Int64Value)(expr);
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                            expr = new Int64Value((Int64)value.Int64);
                        }
                    }
                    break;

                case DataType.DataType_Single:
                    {
                        if (bIsNull)
                        {
                            SingleValue val = new SingleValue();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            DoubleValue value = (DoubleValue)(expr);
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                            expr = new SingleValue((float)value.Double);
                        }
                    }
                    break;

                case DataType.DataType_Double:
                    {
                        if (bIsNull)
                        {
                            DoubleValue val = new DoubleValue();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            DoubleValue value = (DoubleValue)(expr);
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                        }
                    }
                    break;

                case DataType.DataType_DateTime:
                    {
                        if (bIsNull)
                        {
                            DateTimeValue val = new DateTimeValue();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            DateTimeValue value = (DateTimeValue)expr;
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                        }
                    }
                    break;

                case DataType.DataType_Decimal:
                    {
                        if (bIsNull)
                        {
                            DecimalValue val = new DecimalValue();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            DoubleValue valueDouble = (DoubleValue)expr;
                            if (valueDouble != null)
                                expr = new DecimalValue((double)valueDouble.Double);
                            else
                            {
                                Int32Value valueInt32 = (Int32Value)expr;
                                if (valueInt32 != null)
                                    expr = new DecimalValue((double)valueInt32.Int32);
                                else
                                    Debug.Fail("Wrong data type!");
                            }
                        }
                    }
                    break;

                case DataType.DataType_String:
                    {
                        if (bIsNull)
                        {
                            StringValue val = new StringValue();
                            val.SetNull();
                            expr = val;
                        }
                        else
                        {
                            StringValue value = (StringValue)expr;
                            if (value == null)
                            {
                                Debug.Fail("Wrong data type!");
                            }
                        }
                    }
                    break;
                default:
                    Debug.Fail("Unhandled data type!");
                    break;
            }

            return expr;
        }

        public static string expressionToString(Expression expression)
        {
            string val = expression.ToString();
            return val;
        }

        private static DataType dtype;
        public static DataType getDataType(string dataType)
        {
            
            switch (dataType)
            {
                case "Boolean":
                    {
                        dtype = DataType.DataType_Boolean;
                    }
                    break;

                case "Byte":
                    {
                        dtype = DataType.DataType_Byte;
                    }
                    break;

                case "Int16":
                    {
                        dtype = DataType.DataType_Int16;
                    }
                    break;

                case "Int32":
                    {
                        dtype = DataType.DataType_Int32;
                    }
                    break;

                case "Int64":
                    {
                        dtype = DataType.DataType_Int64;
                    }
                    break;

                case "Single":
                    {
                        dtype = DataType.DataType_Single;
                    }
                    break;

                case "Double":
                    {
                        dtype = DataType.DataType_Double;
                    }
                    break;

                case "DateTime":
                    {
                        dtype = DataType.DataType_DateTime;
                    }
                    break;

                case "Decimal":
                    {
                        dtype = DataType.DataType_Decimal;
                    }
                    break;

                case "String":
                    {
                        dtype = DataType.DataType_String;
                    }
                    break;
                default:
                    Debug.Fail("Unhandled data type!");
                    break;
            }
            return dtype;
        }
    }
}

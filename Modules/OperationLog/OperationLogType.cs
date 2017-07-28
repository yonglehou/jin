//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


namespace Tunynet.Logging
{
    public class OperationLogType
    {
        /// <summary>
        /// 同意
        /// </summary>
        public static string Agree(long? name)
        {
            if (name.HasValue)
            {
                return "同意";
            }
            return "Agree";
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        public static string Refuse(long? name)
        {
            if (name.HasValue)
            {
                return "拒绝";
            }
            return "Refuse";
        }

        /// <summary>
        /// 花费
        /// </summary>
        public static string Spend(long? name)
        {
            if (name.HasValue)
            {
                return "消费";
            }
            return "Spend";
        }
        /// <summary>
        /// 充值
        /// </summary>
        public static string Deposit(long? name)
        {
            if (name.HasValue)
            {
                return "充值";
            }
            return "Deposit";
        }
        /// <summary>
        /// 退还
        /// </summary>
        public static string Bank(long? name)
        {
            if (name.HasValue)
            {
                return "退还";
            }
            return "Bank";
        }


        /// <summary>
        /// 启用
        /// </summary>
        public static string Enable(long? name)
        {
            if (name.HasValue)
            {
                return "启用";
            }
            return "Enable";
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public static string Disable(long? name)
        {
            if (name.HasValue)
            {
                return "禁用";
            }
            return "Disable";
        }


        /// <summary>
        /// 创建
        /// </summary>
        public static string Create(long? name)
        {
            if (name.HasValue)
            {
                return "创建";
            }
            return "Create";
        }

        /// <summary>
        /// 编辑
        /// </summary>
        public static string Update(long? name)
        {
            if (name.HasValue)
            {
                return "编辑";
            }
            return "Update";
        }

        /// <summary>
        /// 删除
        /// </summary>
        public static string Delete(long? name)
        {
            if (name.HasValue)
            {
                return "删除";
            }
            return "Delete";
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public static string ResetPassword(long? name)
        {
            if (name.HasValue)
            {
                return "重置密码";
            }
            return "ResetPassword";
        }


        /// <summary>
        /// 显示操作名称
        /// </summary>
        public static string Dispaly(string name)
        {
            switch (name)
            {
                case "DaiDing":
                    return "待定";
                case "ResetPassword":
                    return "重置密码";
                case "Update":
                    return "编辑";
                case "Create":
                    return "创建";
                case "Delete":
                    return "删除";
                case "Disable":
                    return "禁用";
                case "Enable":
                    return "启用";
                case "Spend":
                    return "消费";
                case "Deposit":
                    return "充值";
                case "Refuse":
                    return "拒绝";
                case "Agree":
                    return "同意";
                default:
                    return "";
            }
        }

    }
}

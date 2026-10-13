using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anastasia423WPF
{
    public partial class User
    {
        // Текстовое название роли для интерфейса
        public string RoleName => Role?.Name ?? "Гость";

        // Свойства для разграничения прав в XAML
        public bool IsAdmin => RoleID == 4;
        public bool IsManager => RoleID == 3 || RoleID == 4;
        public bool IsMaster => RoleID == 2;
        public bool IsClient => RoleID == 1;
    }
}

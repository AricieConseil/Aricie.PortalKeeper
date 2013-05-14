using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Security.Roles;

namespace Aricie.DNN.Test.TestFramework.Services.FakesProviders
{
    public class DnnRoleProvider : RoleProvider
    {

        public override bool AddUserToRole(int portalId, DotNetNuke.Entities.Users.UserInfo user, DotNetNuke.Entities.Users.UserRoleInfo userRole)
        {
            throw new NotImplementedException();
        }

        public override bool CreateRole(int portalId, ref RoleInfo role)
        {
            throw new NotImplementedException();
        }

        public override int CreateRoleGroup(RoleGroupInfo roleGroup)
        {
            throw new NotImplementedException();
        }

        public override void DeleteRole(int portalId, ref RoleInfo role)
        {
            throw new NotImplementedException();
        }

        public override void DeleteRoleGroup(RoleGroupInfo roleGroup)
        {
            throw new NotImplementedException();
        }

        public override RoleInfo GetRole(int portalId, string roleName)
        {
            throw new NotImplementedException();
        }

        public override RoleInfo GetRole(int portalId, int roleId)
        {
            throw new NotImplementedException();
        }

        public override RoleGroupInfo GetRoleGroup(int portalId, int roleGroupId)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ArrayList GetRoleGroups(int portalId)
        {
            throw new NotImplementedException();
        }

        public override string[] GetRoleNames(int portalId, int userId)
        {
            throw new NotImplementedException();
        }

        public override string[] GetRoleNames(int portalId)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ArrayList GetRoles(int portalId)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ArrayList GetRolesByGroup(int portalId, int roleGroupId)
        {
            throw new NotImplementedException();
        }

        public override DotNetNuke.Entities.Users.UserRoleInfo GetUserRole(int PortalId, int UserId, int RoleId)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ArrayList GetUserRoles(int PortalId, string Username, string Rolename)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ArrayList GetUserRoles(int PortalId, int UserId, bool includePrivate)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ArrayList GetUserRolesByRoleName(int portalId, string roleName)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ArrayList GetUsersByRoleName(int portalId, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUserFromRole(int portalId, DotNetNuke.Entities.Users.UserInfo user, DotNetNuke.Entities.Users.UserRoleInfo userRole)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRole(RoleInfo role)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRoleGroup(RoleGroupInfo roleGroup)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUserRole(DotNetNuke.Entities.Users.UserRoleInfo userRole)
        {
            throw new NotImplementedException();
        }
    }
}

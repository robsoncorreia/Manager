namespace FC.Domain.Service.Parse
{
    public static class UtilParseResult
    {
        public const string FIELD_CLASSNAME = "className";

        public const string FIELD_CLASSNAME_OLD = "classNameOld";

        public const string FIELD_CREATEDAT = "createdAt";

        public const string FIELD_HOMEADMIN = "homeAdmin";

        //public const string FIELD_PROJECT = "project";
        public const string FIELD_HOMEID = "homeId";

        public const string FIELD_HOMENAME = "homeName";

        public const string FIELD_HOMEUSER = "homeUser";

        public const string FIELD_MANAGERID = "managerId";

        public const string FIELD_MANAGERVERSION = "managerversion";

        public const string FIELD_MANIFESTVERSION = "manifestVersion";

        public const string FIELD_MODULES = "modules";

        public const string FIELD_NEEDSYNC = "needSync";

        public const string FIELD_OBJECTID = "objectId";

        public const string FIELD_OBJECTID_OLD = "objectIdOld";

        public const string FIELD_PARSEID = "parseId";

        // field created by the parse server, you have to tell the old project
        public const string FIELD_PROJECTFILE = "projectFile";

        public const string FIELD_PUSHTOPARSE = "pushToParse";

        public const string FIELD_STATUS = "status";

        // field created by the parse server, you have to tell the old project
        public const string FIELD_TYPE = "type";

        // FIELD PROJECT\RESULT
        public const string FIELD_UPDATEDAT = "updatedAt";

        // error parseserver: old project was written the field as __type
        //relation with class User
        public const string FIELD_USERS = "users";
    }
}
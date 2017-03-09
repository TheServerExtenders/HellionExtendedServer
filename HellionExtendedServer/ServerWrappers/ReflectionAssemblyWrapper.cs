using System;
using System.Reflection;
using HellionExtendedServer.Managers;
using System.Diagnostics;
using HellionExtendedServer.Common;

// Helper class to ease reflecting of the server

namespace HellionExtendedServer.ServerWrappers
{
    public class ReflectionAssemblyWrapper
    {
        #region Fields
        protected static Assembly m_assembly;
        #endregion

        #region Properties
        public static Assembly Assembly { get { return m_assembly; } }
        #endregion

        #region Methods

        public ReflectionAssemblyWrapper(Assembly assembly)
        {
            m_assembly = assembly;
        }
        #endregion
    }

    public abstract class ReflectionClassWrapper
    {
        #region Fields
        protected String m_namespace;
        protected String m_class;
        protected Type m_classType;
        protected Assembly m_assembly;

        #endregion

        #region Properties
        public abstract String ClassName { get; }
        #endregion

        #region Methods
        protected ReflectionClassWrapper(Assembly Assembly, String Namespace)
        {
            m_assembly = Assembly;
            m_namespace = Namespace;
            m_classType = Assembly.GetType(Namespace + "." + ClassName);
        }

        public virtual void Init() { }
        #endregion
    }

    public class ReflectionMember
    {
        #region Fields
        protected Type m_classType;
        protected String m_signature;
        protected String m_className;
        protected MemberInfo[] m_members;
        #endregion

        #region Properties
        public String ClassName { get { return m_className; } }
        public String Signature { get { return m_signature; } }
        #endregion

        #region Methods
        protected ReflectionMember(String signature, String className, Type classType)
        {
            m_signature = signature;
            m_className = className;
            m_classType = classType;
            m_members = classType.GetMember(signature, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (m_members.Length == 0)
            {
                throw new ArgumentException(String.Format("Reflection Error: Signature {0} not found in {1}", signature, className));
            }
        }
        #endregion
    }

    public class ReflectionField : ReflectionMember
    {
        #region Fields
        protected FieldInfo m_field;
        #endregion

        #region Methods
        internal ReflectionField(String signature, String className, Type classType)
            : base(signature, className, classType)
        {
            m_field = classType.GetField(signature, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            if (m_field == null)
            {
                throw new ArgumentException(String.Format("Reflection Error: Field {0} not found in {1}", signature, className));
            }
        }

        public Object GetValue(Object obj)
        {
            return m_field.GetValue(obj);
        }

        public void SetValue(Object obj, Object value)
        {
            m_field.SetValue(obj, value);
        }
        #endregion
    }

    public class ReflectionProperty : ReflectionMember
    {
        #region Fields
        protected PropertyInfo m_property;
        #endregion

        #region Methods
        internal ReflectionProperty(String signature, String className, Type classType)
            : base(signature, className, classType)
        {
            m_property = classType.GetProperty(signature, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);


            if (m_property == null)
            {
                throw new ArgumentException(String.Format("Reflection Error: Property {0} not found in {1}", signature, className));
            }
        }

        public Object GetValue(Object obj)
        {
            return m_property.GetValue(obj);
        }

        public void SetValue(Object obj, Object value)
        {
            m_property.SetValue(obj, value);
        }
        #endregion
    }

    public class ReflectionMethod : ReflectionMember
    {
        #region Methods
        private MethodInfo Get(Object[] parameters)
        {
            Type[] argTypes = new Type[parameters.Length];

            int i = 0;
            foreach (Object arg in parameters)
            {
                argTypes[i] = arg.GetType();
                i++;
            }

            return Get(argTypes);
        }

        private MethodInfo Get(Type[] paramTypes)
        {
            return m_classType.GetMethod(m_signature,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
                null,
                CallingConventions.Standard | CallingConventions.HasThis,
                paramTypes,
                null);
        }

        private MethodInfo Get(Type[] paramTypes, Type genericType)
        {
            MethodInfo[] methods = m_classType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                if (method.Name == Signature)
                {
                    MethodInfo methodInfo = method.MakeGenericMethod(genericType);
                    if (method != null)
                    {
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        int i = 0;
                        int paramcount = paramTypes.Length;
                        bool valid = true;
                        foreach (ParameterInfo parameter in parameters)
                        {
                            if (i == paramcount)
                            {
                                valid = false;
                                break;
                            }
                            if (parameter.ParameterType != paramTypes[i])
                            {
                                valid = false;
                                break;
                            }
                            i++;
                        }
                        if (valid)
                        {
                            return methodInfo;
                        }
                    }
                }
            }

            return null;
        }


        public Object Call(Object obj, Object[] parameters)
        {
            if (parameters == null)
            {
                parameters = new Object[] { };
            }

            MethodInfo methodInfo = Get(parameters);

            if (methodInfo == null)
            {
               Log.Instance.Fatal(String.Format("Overloaded method not found for {0}.{1} with argument types: {2}. Stack Trace: {3}", ClassName, Signature, parameters.ToString(), (new StackTrace()).ToString()));
                return null;
            }

            return methodInfo.Invoke(obj, parameters);
        }

        public Object Call(Object obj, Object[] parameters, Type[] paramTypes)
        {
            if (parameters == null)
            {
                parameters = new Object[] { };
            }

            MethodInfo methodInfo = Get(paramTypes);

            if (methodInfo == null)
            {
                Log.Instance.Fatal(String.Format("Overloaded method not found for {0}.{1} with argument types: {2}. Stack Trace: {3}", ClassName, Signature, parameters.ToString(), (new StackTrace()).ToString()));
                return null;
            }

            return methodInfo.Invoke(obj, parameters);
        }

        public Object Call(Object obj, Object[] parameters, Type[] paramTypes, Type genericType)
        {
            if (parameters == null)
            {
                parameters = new Object[] { };
            }

            MethodInfo methodInfo = Get(paramTypes, genericType);

            if (methodInfo == null)
            {
                Log.Instance.Fatal(String.Format("Overloaded method not found for {0}.{1} with argument types: {2}. Stack Trace: {3}", ClassName, Signature, parameters.ToString(), (new StackTrace()).ToString()));
                return null;
            }

            return methodInfo.Invoke(obj, parameters);
        }

        internal ReflectionMethod(String signature, String className, Type classType)
            : base(signature, className, classType)
        {

        }
        #endregion
    }
}

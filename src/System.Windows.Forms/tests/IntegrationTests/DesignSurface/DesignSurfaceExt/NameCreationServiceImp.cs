using System.ComponentModel.Design.Serialization;
using System.ComponentModel;

//- NameCreationServiceImp - Implementing INameCreationService
//- The INameCreationService interface is used to supply a name to the control just created
//- In the CreateName() we use the same naming algorithm used by Visual Studio: just
//- increment an integer counter until we find a name that isn't already in use.
namespace DesignSurfaceExt
{
    internal class NameCreationServiceImp : INameCreationService
    {
        public NameCreationServiceImp() { }

        public string CreateName(IContainer container, Type type)
        {
            if (null == container)
                return string.Empty;

            ComponentCollection cc = container.Components;
            int min = int.MaxValue;
            int max = int.MinValue;
            int count = 0;

            int i = 0;
            while (i < cc.Count)
            {
                Component comp = cc[i] as Component;

                if (comp.GetType() == type)
                {
                    count++;

                    string name = comp.Site.Name;
                    if (name.StartsWith(type.Name))
                    {
                        try
                        {
                            int value = int.Parse(name.Substring(type.Name.Length));
                            if (value < min)
                                min = value;
                            if (value > max)
                                max = value;
                        }
                        catch (Exception) { }
                    }//end_if
                }//end_if

                i++;
            } //end_while

            if (0 == count)
            {
                return type.Name + "1";
            }
            else if (min > 1)
            {
                int j = min - 1;
                return type.Name + j.ToString();
            }
            else
            {
                int j = max + 1;
                return type.Name + j.ToString();
            }
        }

        public bool IsValidName(string name)
        {
            //- Check that name is "something" and that is a string with at least one char
            if (string.IsNullOrEmpty(name))
                return false;

            //- then the first character must be a letter
            if (!(char.IsLetter(name, 0)))
                return false;

            //- then don't allow a leading underscore
            if (name.StartsWith("_"))
                return false;

            //- ok, it's a valid name
            return true;
        }

        public void ValidateName(string name)
        {
            //-  Use our existing method to check, if it's invalid throw an exception
            if (!(IsValidName(name)))
                throw new ArgumentException("Invalid name: " + name);
        }
    }//end_class
}//end_namespace


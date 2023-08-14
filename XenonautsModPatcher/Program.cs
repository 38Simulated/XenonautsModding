using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //What we need
            // - There lcoation of installed game
            // - Mods folder
            //Check to make sure that the assembly hasn't already been updated (maybe store current verion somewhere?)
            var xenonautsInstallDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Xenonauts2\";
            var modsDirectory = @"%USERPROFILE%\Documents\Xenonauts 2\Mods";


            System.IO.Directory.SetCurrentDirectory(@$"{xenonautsInstallDirectory}\Xenonauts2_Data\Managed");

            ModuleDefinition module = ModuleDefinition.ReadModule("Assembly-CSharp.dll");

            //Check to see if the main DLL has already been patched, if it has just launch the game
            var imod = module.GetType("Xenonauts.XenonautsMain").GetMethods().Where(x => x.FullName.Contains("LoadMods")).FirstOrDefault();

            if(imod != null)
            {
                Console.WriteLine("Dll already patched, opening game");
                Console.ReadLine();
            }

            //Remove existing copy as it'll be an old version
            File.Delete("Assembly-CSharp-ORIG.dll");

            //Make a copy of the main DLL
            File.Copy("Assembly-CSharp.dll", "Assembly-CSharp-ORIG.dll");

            ModuleDefinition system = ModuleDefinition.ReadModule("mscorlib.dll");
            ModuleDefinition systemCore = ModuleDefinition.ReadModule("System.Core.dll");

            // .net framework 3.5 types from DLL included with Xenonauts
            var expandEnvironmentVariables = module.ImportReference(system.GetType("System.Environment").GetMethods().Where(x => x.FullName.Contains("System.Environment::ExpandEnvironmentVariables(System.String)")).First());
            var getDirectories = module.ImportReference(system.GetType("System.IO.Directory").GetMethods().Where(x => x.FullName.Contains("System.IO.Directory::GetDirectories(System.String)")).First());
            var GetFileName = module.ImportReference(system.GetType("System.IO.Path").GetMethods().Where(x => x.FullName.Contains("System.IO.Path::GetFileName")).First());
            var concat = module.ImportReference(system.GetType("System.String").GetMethods().Where(x => x.IsStatic && x.FullName.Contains("System.String::Concat(System.String,System.String)")).First());
            var Exists = module.ImportReference(system.GetType("System.IO.File").GetMethods().Where(x => x.IsStatic && x.FullName.Contains("System.IO.File::Exists(System.String)")).First());
            TypeReference SystemType = module.ImportReference(system.GetType("System.Type"));
            TypeReference SystemType2 = module.ImportReference(system.GetType("System.Type"));
            var op_Equality = module.ImportReference(system.GetType("System.String").GetMethods().Where(x => x.FullName.Contains("System.String::op_Equality(System.String,System.String)")).First());
            var op_Equality2 = module.ImportReference(system.GetType("System.String").GetMethods().Where(x => x.FullName.Contains("System.String::op_Equality(System.String,System.String)")).First());
            var GetMethod = module.ImportReference(system.GetType("System.Type").GetMethods().Where(x => x.FullName.Contains("System.Type::GetMethod(System.String)")).First());
            var MethodInfo = module.ImportReference(system.GetType("System.Type"));
            var Invoke = module.ImportReference(system.GetType("System.Reflection.MethodBase").GetMethods().Where(x => x.FullName.Contains("System.Reflection.MethodBase::Invoke(System.Object,System.Object[])")).First());
            var CreateInstance2 = module.ImportReference(system.GetType("System.Activator").GetMethods().Where(x => x.FullName.Contains("System.Activator::CreateInstance(System.Type)")).First());
            var CreateInstance = module.ImportReference(system.GetType("System.Activator").GetMethods().Where(x => x.FullName.Contains("System.Activator::CreateInstance(System.Type)")).First());
            var get_IsClass = module.ImportReference(system.GetType("System.Type").GetMethods().Where(x => x.FullName.Contains("System.Type::get_IsClass")).First());
            var GetTypeFromHandle = module.ImportReference(system.GetType("System.Type").GetMethods().Where(x => x.FullName.Contains("System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)")).First());
            var IsAssignableFrom = module.ImportReference(system.GetType("System.Type").GetMethods().Where(x => x.FullName.Contains("System.Type::IsAssignableFrom(System.Type)")).First());
            var LoadFile = module.ImportReference(system.GetType("System.Reflection.Assembly").GetMethods().Where(x => x.IsStatic && x.FullName.Contains("System.Reflection.Assembly::LoadFile(System.String)")).First());
            var GetExportedTypes = module.ImportReference(system.GetType("System.Reflection.Assembly").GetMethods().Where(x => x.FullName.Contains("System.Reflection.Assembly::GetExportedTypes")).First());
            TypeReference SystemType3 = module.ImportReference(system.GetType("System.Type"));

            var Repeat = module.ImportReference(systemCore.GetType("System.Linq.Enumerable").GetMethods().Where(x => x.FullName.Contains("System.Linq.Enumerable::Repeat")).First());
            var ToArray = module.ImportReference(systemCore.GetType("System.Linq.Enumerable").GetMethods().Where(x => x.FullName.Contains("System.Linq.Enumerable::ToArray")).First());


            //Add IMod interface to main DLL
            var itf_IMod_0 = new TypeDefinition("Xenonauts", "IMod", TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);
            module.Types.Add(itf_IMod_0);

            //Get the XenonautsMain class ready to add new LoadMods method and update contructor 
            TypeDefinition classToAddTo = module.Types.Where(x => x.FullName == "Xenonauts.XenonautsMain").First();
            var cls_To_Add_To_0 = classToAddTo;

            //Start creation of LoadMods method
            //Method : LoadMods
            var md_LoadMods_2 = new MethodDefinition("LoadMods", MethodAttributes.Public | MethodAttributes.HideBySig, module.TypeSystem.Void);
            cls_To_Add_To_0.Methods.Add(md_LoadMods_2);
            md_LoadMods_2.Body.InitLocals = true;
            var il_LoadMods_3 = md_LoadMods_2.Body.GetILProcessor();

            //string[] directories = Directory.GetDirectories(Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Documents\\Xenonauts 2\\Mods"));
            var lv_directories_4 = new VariableDefinition(system.TypeSystem.String.MakeArrayType());
            md_LoadMods_2.Body.Variables.Add(lv_directories_4);
            il_LoadMods_3.Emit(OpCodes.Ldstr, modsDirectory);
            il_LoadMods_3.Emit(OpCodes.Call, expandEnvironmentVariables);
            il_LoadMods_3.Emit(OpCodes.Call, getDirectories);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_directories_4);

            var lv_dirsThatExist_5 = new VariableDefinition(module.TypeSystem.String.MakeArrayType());
            md_LoadMods_2.Body.Variables.Add(lv_dirsThatExist_5);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 1000);
            il_LoadMods_3.Emit(OpCodes.Newarr, module.TypeSystem.String);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_dirsThatExist_5);

            //for (int i = 0; i < dirsThatExist.Length; i++)...
            var lv_i_6 = new VariableDefinition(module.TypeSystem.Int32);
            md_LoadMods_2.Body.Variables.Add(lv_i_6);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 0);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_i_6);
            var lbl_fel_7 = il_LoadMods_3.Create(OpCodes.Nop);
            var Nop_8 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(Nop_8);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_6);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_dirsThatExist_5);
            il_LoadMods_3.Emit(OpCodes.Ldlen);
            il_LoadMods_3.Emit(OpCodes.Conv_I4);
            il_LoadMods_3.Emit(OpCodes.Clt);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_fel_7);

            //dirsThatExist[i] = "";
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_dirsThatExist_5);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_6);
            il_LoadMods_3.Emit(OpCodes.Ldstr, "");
            il_LoadMods_3.Emit(OpCodes.Stelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_6);
            il_LoadMods_3.Emit(OpCodes.Dup);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4_1);
            il_LoadMods_3.Emit(OpCodes.Add);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_i_6);
            il_LoadMods_3.Emit(OpCodes.Pop);
            il_LoadMods_3.Emit(OpCodes.Br, Nop_8);
            il_LoadMods_3.Append(lbl_fel_7);

            //int count = 0;
            var lv_count_6 = new VariableDefinition(module.TypeSystem.Int32);
            md_LoadMods_2.Body.Variables.Add(lv_count_6);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 0);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_count_6);

            //for (int i = 0; i < directories.Length; i++)...
            var lv_i_7 = new VariableDefinition(module.TypeSystem.Int32);
            md_LoadMods_2.Body.Variables.Add(lv_i_7);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 0);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_i_7);
            var lbl_fel_8 = il_LoadMods_3.Create(OpCodes.Nop);
            var Nop_9 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(Nop_9);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_7);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_directories_4);
            il_LoadMods_3.Emit(OpCodes.Ldlen);
            il_LoadMods_3.Emit(OpCodes.Conv_I4);
            il_LoadMods_3.Emit(OpCodes.Clt);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_fel_8);

            //string fileName = Path.GetFileName(directories[i]);
            var lv_fileName_10 = new VariableDefinition(module.TypeSystem.String);
            md_LoadMods_2.Body.Variables.Add(lv_fileName_10);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_directories_4);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_7);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Call, GetFileName);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_fileName_10);

            //string path = directories[i] + "\\" + fileName + ".dll";
            var lv_path_11 = new VariableDefinition(module.TypeSystem.String);
            md_LoadMods_2.Body.Variables.Add(lv_path_11);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_directories_4);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_7);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Ldstr, "\\\\");
            il_LoadMods_3.Emit(OpCodes.Call, concat);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_fileName_10);
            il_LoadMods_3.Emit(OpCodes.Call, concat);
            il_LoadMods_3.Emit(OpCodes.Ldstr, ".dll");
            il_LoadMods_3.Emit(OpCodes.Call, concat);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_path_11);

            //if (File.Exists(path))...
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_path_11);
            il_LoadMods_3.Emit(OpCodes.Call, Exists);
            var lbl_elseEntryPoint_12 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_elseEntryPoint_12);
            //if body

            //dirsThatExist[count] = path;
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_dirsThatExist_5);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_count_6);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_path_11);
            il_LoadMods_3.Emit(OpCodes.Stelem_Ref);

            //count++;
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_count_6);
            il_LoadMods_3.Emit(OpCodes.Dup);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4_1);
            il_LoadMods_3.Emit(OpCodes.Add);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_count_6);
            il_LoadMods_3.Emit(OpCodes.Pop);
            var lbl_elseEnd_13 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(lbl_elseEntryPoint_12);
            il_LoadMods_3.Append(lbl_elseEnd_13);
            md_LoadMods_2.Body.OptimizeMacros();
            // end if (if (File.Exists(path))...)
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_7);
            il_LoadMods_3.Emit(OpCodes.Dup);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4_1);
            il_LoadMods_3.Emit(OpCodes.Add);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_i_7);
            il_LoadMods_3.Emit(OpCodes.Pop);
            il_LoadMods_3.Emit(OpCodes.Br, Nop_9);
            il_LoadMods_3.Append(lbl_fel_8);

            //Type[] mods = new Type[1000];
            var lv_mods_14 = new VariableDefinition(SystemType);
            md_LoadMods_2.Body.Variables.Add(lv_mods_14);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 1000);
            il_LoadMods_3.Emit(OpCodes.Newarr, SystemType2);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_mods_14);

            //count = 0;
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 0);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_count_6);

            //for (int i = 0; i < dirsThatExist.Length; i++)...
            var lv_i_15 = new VariableDefinition(module.TypeSystem.Int32);
            md_LoadMods_2.Body.Variables.Add(lv_i_15);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 0);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_i_15);
            var lbl_fel_16 = il_LoadMods_3.Create(OpCodes.Nop);
            var Nop_17 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(Nop_17);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_15);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_dirsThatExist_5);
            il_LoadMods_3.Emit(OpCodes.Ldlen);
            il_LoadMods_3.Emit(OpCodes.Conv_I4);
            il_LoadMods_3.Emit(OpCodes.Clt);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_fel_16);

            //if (dirsThatExist[i] == null)...
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_dirsThatExist_5);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_15);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Ldstr, "");
            il_LoadMods_3.Emit(OpCodes.Call, op_Equality);

            var lbl_elseEntryPoint_18 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_elseEntryPoint_18);
            //if body

            //break;
            il_LoadMods_3.Emit(OpCodes.Br, lbl_fel_16);
            var lbl_elseEnd_19 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(lbl_elseEntryPoint_18);
            il_LoadMods_3.Append(lbl_elseEnd_19);
            md_LoadMods_2.Body.OptimizeMacros();
            // end if (if (dirsThatExist[i] == null)...)

            //Type[] exportedTypes = Assembly.LoadFile(dirsThatExist[i]).GetExportedTypes();
            var lv_exportedTypes_20 = new VariableDefinition(SystemType3.MakeArrayType());
            md_LoadMods_2.Body.Variables.Add(lv_exportedTypes_20);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_dirsThatExist_5);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_15);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Call, LoadFile);
            il_LoadMods_3.Emit(OpCodes.Callvirt, GetExportedTypes);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_exportedTypes_20);

            //for (int j = 0; j < exportedTypes.Length; j++)...
            var lv_j_21 = new VariableDefinition(module.TypeSystem.Int32);
            md_LoadMods_2.Body.Variables.Add(lv_j_21);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 0);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_j_21);
            var lbl_fel_22 = il_LoadMods_3.Create(OpCodes.Nop);
            var Nop_23 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(Nop_23);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_j_21);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_exportedTypes_20);
            il_LoadMods_3.Emit(OpCodes.Ldlen);
            il_LoadMods_3.Emit(OpCodes.Conv_I4);
            il_LoadMods_3.Emit(OpCodes.Clt);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_fel_22);

            //if (exportedTypes[j].IsClass && typeof(IMod).IsAssignableFrom(exportedTypes[j]))...
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_exportedTypes_20);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_j_21);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Callvirt, get_IsClass);
            il_LoadMods_3.Emit(OpCodes.Ldtoken, itf_IMod_0);
            il_LoadMods_3.Emit(OpCodes.Call, GetTypeFromHandle);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_exportedTypes_20);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_j_21);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Callvirt, IsAssignableFrom);
            il_LoadMods_3.Emit(OpCodes.And);
            var lbl_elseEntryPoint_24 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_elseEntryPoint_24);
            //if body

            //mods[count] = exportedTypes[j];
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_mods_14);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_count_6);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_exportedTypes_20);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_j_21);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Stelem_Ref);

            //count++;
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_count_6);
            il_LoadMods_3.Emit(OpCodes.Dup);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4_1);
            il_LoadMods_3.Emit(OpCodes.Add);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_count_6);
            il_LoadMods_3.Emit(OpCodes.Pop);
            var lbl_elseEnd_25 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(lbl_elseEntryPoint_24);
            il_LoadMods_3.Append(lbl_elseEnd_25);
            md_LoadMods_2.Body.OptimizeMacros();
            // end if (if (exportedTypes[j].IsClass && typeof(IMod).IsAssignableFrom(exportedTypes[j]))...)
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_j_21);
            il_LoadMods_3.Emit(OpCodes.Dup);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4_1);
            il_LoadMods_3.Emit(OpCodes.Add);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_j_21);
            il_LoadMods_3.Emit(OpCodes.Pop);
            il_LoadMods_3.Emit(OpCodes.Br, Nop_23);
            il_LoadMods_3.Append(lbl_fel_22);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_15);
            il_LoadMods_3.Emit(OpCodes.Dup);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4_1);
            il_LoadMods_3.Emit(OpCodes.Add);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_i_15);
            il_LoadMods_3.Emit(OpCodes.Pop);
            il_LoadMods_3.Emit(OpCodes.Br, Nop_17);
            il_LoadMods_3.Append(lbl_fel_16);

            //for (int i = 0; i < mods.Length; i++)...
            var lv_i_26 = new VariableDefinition(module.TypeSystem.Int32);
            md_LoadMods_2.Body.Variables.Add(lv_i_26);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4, 0);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_i_26);
            var lbl_fel_27 = il_LoadMods_3.Create(OpCodes.Nop);
            var Nop_28 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(Nop_28);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_26);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_mods_14);
            il_LoadMods_3.Emit(OpCodes.Ldlen);
            il_LoadMods_3.Emit(OpCodes.Conv_I4);
            il_LoadMods_3.Emit(OpCodes.Clt);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_fel_27);

            //if (mods[i] == null)...
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_mods_14);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_26);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Ldnull);
            il_LoadMods_3.Emit(OpCodes.Call, op_Equality2);
            var lbl_elseEntryPoint_29 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Emit(OpCodes.Brfalse, lbl_elseEntryPoint_29);
            //if body

            //break;
            il_LoadMods_3.Emit(OpCodes.Br, lbl_fel_27);
            var lbl_elseEnd_30 = il_LoadMods_3.Create(OpCodes.Nop);
            il_LoadMods_3.Append(lbl_elseEntryPoint_29);
            il_LoadMods_3.Append(lbl_elseEnd_30);
            md_LoadMods_2.Body.OptimizeMacros();
            // end if (if (mods[i] == null)...)

            //IMod mod = (IMod)Activator.CreateInstance(mods[i]);
            var lv_mod_31 = new VariableDefinition(itf_IMod_0);
            md_LoadMods_2.Body.Variables.Add(lv_mod_31);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_mods_14);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_26);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Call, CreateInstance);
            il_LoadMods_3.Emit(OpCodes.Castclass, itf_IMod_0);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_mod_31);

            //object obj = Activator.CreateInstance(mods[i]);
            var lv_obj_32 = new VariableDefinition(module.TypeSystem.Object);
            md_LoadMods_2.Body.Variables.Add(lv_obj_32);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_mods_14);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_26);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Call, CreateInstance2);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_obj_32);

            //MethodInfo method = mods[i].GetMethod("Initialise");
            var lv_method_33 = new VariableDefinition(MethodInfo);
            md_LoadMods_2.Body.Variables.Add(lv_method_33);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_mods_14);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_26);
            il_LoadMods_3.Emit(OpCodes.Ldelem_Ref);
            il_LoadMods_3.Emit(OpCodes.Ldstr, "Initialise");
            il_LoadMods_3.Emit(OpCodes.Callvirt, GetMethod);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_method_33);

            //method.Invoke(obj, null);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_method_33);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_obj_32);
            il_LoadMods_3.Emit(OpCodes.Ldnull);
            il_LoadMods_3.Emit(OpCodes.Callvirt, Invoke);
            il_LoadMods_3.Emit(OpCodes.Pop);
            il_LoadMods_3.Emit(OpCodes.Ldloc, lv_i_26);
            il_LoadMods_3.Emit(OpCodes.Dup);
            il_LoadMods_3.Emit(OpCodes.Ldc_I4_1);
            il_LoadMods_3.Emit(OpCodes.Add);
            il_LoadMods_3.Emit(OpCodes.Stloc, lv_i_26);
            il_LoadMods_3.Emit(OpCodes.Pop);
            il_LoadMods_3.Emit(OpCodes.Br, Nop_28);
            il_LoadMods_3.Append(lbl_fel_27);
            il_LoadMods_3.Emit(OpCodes.Ret);

            module.Write("Assembly-CSharp-patched.dll");
            module.Dispose();

            File.Delete("Assembly-CSharp.dll");

            //Set up XenonautsMain contructor to call LoadMods
            ModuleDefinition module2 = ModuleDefinition.ReadModule("Assembly-CSharp-patched.dll");
            var LoadModsMethod = module2.GetType("Xenonauts.XenonautsMain").GetMethods().Where(x => x.FullName.Contains("Xenonauts.XenonautsMain::LoadMods")).First();
            var constructor = module2.GetType("Xenonauts.XenonautsMain").GetConstructors().Where(x => !x.IsStatic).First();
            var processor = constructor.Body.GetILProcessor();

            var newInstruction = processor.Create(OpCodes.Call, LoadModsMethod);
            var newInstruction2 = processor.Create(OpCodes.Ldarg_0);

            var firstInstruction = constructor.Body.Instructions[constructor.Body.Instructions.Count-2];
            processor.InsertBefore(firstInstruction, newInstruction);

            var firstInstruction2 = constructor.Body.Instructions[constructor.Body.Instructions.Count - 2];
            processor.InsertBefore(firstInstruction2, newInstruction2);

            module2.Write("Assembly-CSharp.dll");
            module2.Dispose();
            File.Delete("Assembly-CSharp-patched.dll");

        }
    }


}

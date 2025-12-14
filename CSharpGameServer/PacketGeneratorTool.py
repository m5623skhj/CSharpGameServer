import yaml
import os

def IsValidPacketTypeInYaml(yamlData):
    returnValue = True
    checkedInvalidUniqueType = 0
    uniqueTypePacketName = ''
    duplicateChecker = set()
    
    for value in yamlData:
        packetType = value['Type']
        packetName = value['PacketName']
        
        if packetType == 'Unique':
            if checkedInvalidUniqueType == 0:
                checkedInvalidUniqueType += 1
                uniqueTypePacketName = packetName
                duplicateChecker.add(packetName)
                continue
            else:
                checkedInvalidUniqueType += 1
                print("Duplicated Unique type " + uniqueTypePacketName + " and " + packetName)
                returnValue = False
        
        if packetType != 'RequestPacket' and packetType != 'ReplyPacket':                
            print("Invalid packet type : PacketName " + packetName + " / Type : " + value['Type'])
            returnValue = False
            continue
            
        if packetName in duplicateChecker:
            print("Duplicate packet name : " + packetName)
            returnValue = False
            continue
    
        duplicateChecker.add(packetName)
    
    return returnValue

def GenerateEnumValue(packetName, values, namespace):
    enumCode = f"namespace {namespace}\n"
    enumCode += "{\n"
    enumCode += f"    public enum {packetName}\n    {{\n"
    for value in values:
        enumCode += f"        {value['PacketName']},\n"
    enumCode += "    }\n}\n"
    
    return enumCode

def GenerateProtocolOverride(values, namespace, isServer=True):
    generateCode = "#nullable disable\n\n"
    
    if isServer:
        generateCode += "using CSharpGameServer.Core;\n"
        generateCode += "using CSharpGameServer.PacketBase;\n"
        generateCode += "using System.Runtime.InteropServices;\n\n"
    else:
        generateCode += "using System.Runtime.InteropServices;\n\n"
    
    generateCode += f"namespace {namespace}\n{{\n"

    for value in values:
        packetType = value['Type']
        if packetType != 'RequestPacket' and packetType != 'ReplyPacket':
            continue

        fields = value.get("Fields", [])
        packetName = value['PacketName']
        
        generateCode += "    [StructLayout(LayoutKind.Sequential, Pack = 1)]\n"
        generateCode += f"    public struct {packetName}Data\n"
        generateCode += "    {\n"
        generateCode += "        public int PacketType;\n"
        generateCode += "        public ushort PacketSize;\n"
        
        for field in fields:
            fieldName = field["Name"]
            fieldType = field["Type"]
            arraySize = field.get("ArraySize")
            
            if arraySize:
                generateCode += f"        public unsafe fixed {fieldType} {fieldName}[{arraySize}];\n"
            else:
                generateCode += f"        public {fieldType} {fieldName};\n"
        
        generateCode += "    }\n\n"
        
        if isServer:
            generateCode += f"    public class {packetName}Packet : {packetType}\n"
            generateCode += "    {\n"
            generateCode += f"        public {packetName}Data Data;\n\n"
            
            generateCode += "        public override void SetPacketType()\n"
            generateCode += "        {\n"
            generateCode += f"            Type = PacketType.{packetName};\n"
            generateCode += "        }\n\n"

            if packetType == 'RequestPacket':
                generateCode += "        protected override Action<Client, RequestPacket> GetHandler()\n"
                generateCode += "        {\n"
                generateCode += f"            return PacketHandlerManager.Handle{packetName};\n"
                generateCode += "        }\n\n"

            generateCode += "        public override void LoadFromBytes(byte[] buffer, int offset, ushort length)\n"
            generateCode += "        {\n"
            generateCode += f"            var size = Marshal.SizeOf<{packetName}Data>();\n"
            generateCode += "            if (length < size)\n"
            generateCode += "            {\n"
            generateCode += f"                Logger.LoggerManager.Instance.WriteLogError(\"{packetName} packet length {{length}} < struct size {{size}}\", length, size);\n"
            generateCode += "                return;\n"
            generateCode += "            }\n\n"
            generateCode += "            if (offset + size > buffer.Length)\n"
            generateCode += "            {\n"
            generateCode += f"                Logger.LoggerManager.Instance.WriteLogError(\"{packetName} buffer overflow: offset={{offset}}, size={{size}}, bufferLength={{bufferLength}}\", offset, size, buffer.Length);\n"
            generateCode += "                return;\n"
            generateCode += "            }\n\n"
            generateCode += "            var ptr = Marshal.AllocHGlobal(size);\n"
            generateCode += "            try\n"
            generateCode += "            {\n"
            generateCode += "                Marshal.Copy(buffer, offset, ptr, size);\n"
            generateCode += f"                Data = Marshal.PtrToStructure<{packetName}Data>(ptr);\n"
            generateCode += "            }\n"
            generateCode += "            finally\n"
            generateCode += "            {\n"
            generateCode += "                Marshal.FreeHGlobal(ptr);\n"
            generateCode += "            }\n"
            generateCode += "        }\n"
            
            generateCode += "\n        public override byte[] ToBytes()\n"
            generateCode += "        {\n"
            generateCode += f"            var size = Marshal.SizeOf<{packetName}Data>();\n"
            generateCode += f"            Data.PacketType = (int)PacketType.{packetName};\n"
            generateCode += "            Data.PacketSize = (ushort)size;\n\n"
            generateCode += "            var buffer = new byte[size];\n"
            generateCode += "            var ptr = Marshal.AllocHGlobal(size);\n"
            generateCode += "            try\n"
            generateCode += "            {\n"
            generateCode += "                Marshal.StructureToPtr(Data, ptr, false);\n"
            generateCode += "                Marshal.Copy(ptr, buffer, 0, size);\n"
            generateCode += "            }\n"
            generateCode += "            finally\n"
            generateCode += "            {\n"
            generateCode += "                Marshal.FreeHGlobal(ptr);\n"
            generateCode += "            }\n"
            generateCode += "            return buffer;\n"
            generateCode += "        }\n"
            generateCode += "    }\n\n"

    generateCode += "}"
    return generateCode

def GeneratePacketHandler(values):
    generateCode = "using CSharpGameServer.Core;\n"
    generateCode += "using CSharpGameServer.PacketBase;\n\n"
    generateCode += "namespace CSharpGameServer.Packet\n{\n"
    generateCode += "    public partial class PacketHandlerManager\n    {\n"
    
    for value in values:
        if value['Type'] != 'RequestPacket':
            continue

        packetName = value['PacketName']
        generateCode += f"        public static void Handle{packetName}(Client client, RequestPacket packet)\n"
        generateCode += "        {\n"
        generateCode += f"            if (packet is not {packetName}Packet {packetName.lower()}packet)\n"
        generateCode += "            {\n"
        generateCode += "                return;\n"
        generateCode += "            }\n\n"
        generateCode += f"            client.Handle{packetName}({packetName.lower()}packet);\n"
        generateCode += "        }\n\n"

    generateCode += "    }\n}"
    return generateCode

def GenerateClientPacketHandler(values):
    generateCode = "using CSharpGameServer.Packet;\n\n"
    generateCode += "namespace CSharpGameServer.Core\n{\n"
    generateCode += "    public partial class Client\n    {\n"

    for value in values:
        packetType = value['Type']
        if packetType != 'RequestPacket':
            continue
        
        packetName = value['PacketName']
        generateCode += f"        public virtual void Handle{packetName}({packetName}Packet {packetName.lower()}packet) {{ }}\n"

    generateCode += "    }\n"
    generateCode += "}"
    return generateCode

def GenerateClientPacketHeader():
    code = "using System.Runtime.InteropServices;\n\n"
    code += "namespace CSharpGameServer\n{\n"
    code += "    [StructLayout(LayoutKind.Sequential, Pack = 1)]\n"
    code += "    public struct PacketHeader\n"
    code += "    {\n"
    code += "        public int PacketType;\n"
    code += "        public ushort PacketSize;\n"
    code += "    }\n"
    code += "}\n"
    return code

def GenerateClientProtocol(values, namespace):
    code = "using System.Runtime.InteropServices;\n\n"
    code += f"namespace {namespace}\n{{\n"
    
    code += "    [StructLayout(LayoutKind.Sequential, Pack = 1)]\n"
    code += "    public struct PacketHeader\n"
    code += "    {\n"
    code += "        public int PacketType;\n"
    code += "        public ushort PacketSize;\n"
    code += "    }\n\n"
    
    for value in values:
        packetType = value['Type']
        if packetType != 'RequestPacket' and packetType != 'ReplyPacket':
            continue

        fields = value.get("Fields", [])
        packetName = value['PacketName']
        
        code += "    [StructLayout(LayoutKind.Sequential, Pack = 1)]\n"
        code += f"    public struct {packetName}Packet\n"
        code += "    {\n"
        code += "        public PacketHeader Header;\n"
        
        for field in fields:
            fieldName = field["Name"]
            fieldType = field["Type"]
            arraySize = field.get("ArraySize")
            
            if arraySize:
                code += f"        public unsafe fixed {fieldType} {fieldName}[{arraySize}];\n"
            else:
                code += f"        public {fieldType} {fieldName};\n"
        
        code += "    }\n\n"
    
    code += "}"
    return code

def EnsureDirectoryExists(filePath):
    directory = os.path.dirname(filePath)
    if directory and not os.path.exists(directory):
        os.makedirs(directory)

def ProcessPacketGenerate():
    with open(ymlFilePath, 'r') as file:
        ymlData = yaml.load(file, Loader=yaml.SafeLoader)
    
    if IsValidPacketTypeInYaml(ymlData['Packet']) == False:
        print("Code generate failed")
        exit()
    
    packetList = ymlData['Packet']
    
    # Generate Server PacketType.cs
    enumCode = GenerateEnumValue('PacketType', packetList, 'CSharpGameServer.Packet')
    EnsureDirectoryExists(serverPacketTypeFilePath)
    with open(serverPacketTypeFilePath, 'w') as file:
        file.write(enumCode)
    print(f"Generated: {serverPacketTypeFilePath}")

    # Generate Server Protocol.cs
    EnsureDirectoryExists(serverProtocolFilePath)
    with open(serverProtocolFilePath, 'w') as file:
        file.write(GenerateProtocolOverride(packetList, 'CSharpGameServer.Packet', isServer=True))
    print(f"Generated: {serverProtocolFilePath}")
    
    # Generate PacketHandler.cs
    EnsureDirectoryExists(serverPacketHandlerFilePath)
    with open(serverPacketHandlerFilePath, 'w') as file:
        file.write(GeneratePacketHandler(packetList))
    print(f"Generated: {serverPacketHandlerFilePath}")
            
    # Generate ClientPacketHandler.cs
    EnsureDirectoryExists(serverClientPacketHandlerFilePath)
    with open(serverClientPacketHandlerFilePath, 'w') as file:
        file.write(GenerateClientPacketHandler(packetList))
    print(f"Generated: {serverClientPacketHandlerFilePath}")
    
    # Generate TestClient PacketType.cs
    enumCode = GenerateEnumValue('PacketType', packetList, 'CSharpGameServer')
    EnsureDirectoryExists(clientPacketTypeFilePath)
    with open(clientPacketTypeFilePath, 'w') as file:
        file.write(enumCode)
    print(f"Generated: {clientPacketTypeFilePath}")
    
    # Generate TestClient Protocol.cs (with PacketHeader)
    EnsureDirectoryExists(clientProtocolFilePath)
    with open(clientProtocolFilePath, 'w') as file:
        file.write(GenerateClientProtocol(packetList, 'CSharpGameServer'))
    print(f"Generated: {clientProtocolFilePath}")

# Server paths
serverPacketTypeFilePath = 'CSharpGameServer/Packet/PacketType.cs'
serverProtocolFilePath = 'CSharpGameServer/Packet/Protocol.cs'
serverPacketHandlerFilePath = 'CSharpGameServer/Packet/PacketHandler.cs'
serverClientPacketHandlerFilePath = 'CSharpGameServer/Core/ClientPacketHandler.cs'

# TestClient paths
clientPacketTypeFilePath = 'TestClient/Protocol/PacketType.cs'
clientProtocolFilePath = 'TestClient/Protocol/Protocol.cs'

# YAML path
ymlFilePath = 'PacketDefine.yml'

ProcessPacketGenerate()
print("\n=== Code generated successfully ===")
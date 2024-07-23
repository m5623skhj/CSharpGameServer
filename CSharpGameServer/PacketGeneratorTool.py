import yaml

def IsValidPacketTypeInYaml(yamlData):
    returnValue = True
    checkedInvalidUniqueType = 0
    
    for value in yamlData:
        packetType = value['Type']
        
        if packetType == 'UniqueType':
            if checkedInvalidUniqueType == 0:
                checkedInvalidUniqueType += 1
                continue
            else:
                checkedInvalidUniqueType += 1
                print("Duplicated UniqueType type " + value['PacketName'])
                returnValue = False
        
        if packetType != 'RequestPacket' and packetType != 'ReplyPacket':
            print("Invalid packet type : PacketName " + value['PacketName'] + " / Type : " + value['Type'])
            returnValue = False
    
    return returnValue
        

def GenerateEnumValue(packetName, values):
    enumCode = f"namespace CSharpGameServer\n"
    enumCode += "{\n"
    enumCode += f"    public enum {packetName} : int\n    {{\n"
    for value in values:
        enumCode += f"        {value['PacketName']},\n"
    enumCode += "    }\n}\n"
    
    return enumCode
        
def GenerateProtocolOverride(values):
    generateCode = "using CSharpGameServer.Core;\nusing CSharpGameServer.Protocol;\n\n"
    generateCode += "namespace CSharpGameServer.Packet\n{\n"

    for value in values:
        packetType = value['Type']
        if packetType != 'RequestPacket' and packetType != 'ReplyPacket' :
            continue

        packetName = value['PacketName']
        if packetType == 'RequestPacket':
            generateCode += f"    public partial class {packetName} : {packetType}\n"
        else :
            generateCode += f"    public class {packetName} : {packetType}\n"
        generateCode += "    {\n"
        generateCode += "        public override void SetPacketType()\n"
        generateCode += "        {\n"
        generateCode += f"            type = PacketType.{packetName};\n"
        generateCode += "        }\n"

        if packetType == 'RequestPacket':
            generateCode += "        protected override Action<Client, RequestPacket> GetHandler()\n"
            generateCode += "        {\n"
            generateCode += f"            return PacketHandlerManager.Handle{packetName};\n"
            generateCode += "        }\n"

        generateCode += "    }\n\n"

    generateCode += "}"
    return generateCode

def GeneratePacketHandler(values):
    generateCode = "using CSharpGameServer.Core;\nusing CSharpGameServer.Protocol;\n\n"
    generateCode += "namespace CSharpGameServer.Packet\n{\n"
    generateCode += "    public partial class PacketHandlerManager\n    {\n"
    
    for value in values:
        if value['Type'] != 'RequestPacket':
            continue

        generateCode += f"        public static void Handle{value['PacketName']}(Client client, RequestPacket packet)\n"
        generateCode += "        {\n"
        generateCode += f"            {value['PacketName']}? {value['PacketName'].lower()} = packet as {value['PacketName']};\n"
        generateCode += f"            if ({value['PacketName'].lower()} == null)\n"
        generateCode += f"                return;\n\n"
        generateCode += f"            client.Handle{value['PacketName']}({value['PacketName'].lower()});\n"
        generateCode += "        }\n\n"

    generateCode += "    }\n}"
    return generateCode

def GenerateClientPacketHandler(values):
    generateCode = "using CSharpGameServer.Packet;\n\n"
    generateCode += "namespace CSharpGameServer.Core\n{\n"
    generateCode += "    public partial class Client\n"

    for value in values:
        packetType = value['Type']
        if packetType != 'RequestPacket':
            continue
        
        packetName = value['PacketName']
        generateCode += "    {\n"
        generateCode += f"        public virtual void Handle{packetName}({packetName} {packetName.lower()}) {{ }}\n"
        generateCode += "    }\n"

    generateCode += "}"
    return generateCode

def ProcessPacketGenerate():
    with open(ymlFilePath, 'r') as file:
        ymlData = yaml.load(file, Loader=yaml.SafeLoader)
    
    if IsValidPacketTypeInYaml(ymlData['Packet']) == False:
        print("Code generate failed")
        exit()
    
    packetList = ymlData['Packet']
    # Generate PacketType.cs
    enumCode = GenerateEnumValue('PacketType', packetList)
    with open(packetTypeFilePath, 'w') as file:
        file.write(enumCode)
    

    # # Generate Protocol.cs
    with open(protocolFilePath, 'w') as file:
        file.write(GenerateProtocolOverride(packetList))
    
    # Generate PacketHandler.cs
    with open(packetHandlerFilePath, 'w') as file:
        file.write(GeneratePacketHandler(packetList))
            
    # # Generate ClientPacketHandler.cs
    with open(clientPacketHandlerFilePath, 'w') as file:
        file.write(GenerateClientPacketHandler(packetList))


# Write file path here
packetTypeFilePath = 'CSharpGameServer/Packet/PacketType.cs'
protocolFilePath = 'CSharpGameServer/Packet/Protocol.cs'
packetHandlerFilePath = 'CSharpGameServer/Packet/PacketHandler.cs'
clientPacketHandlerFilePath = 'CSharpGameServer/Core/ClientPacketHandler.cs'
ymlFilePath = 'PacketDefine.yml'

ProcessPacketGenerate()
print("Code generated successfully")
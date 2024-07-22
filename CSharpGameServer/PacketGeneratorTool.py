import yaml

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
        if packetType == 'RequestPacket' :
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

        generateCode += "    }\n"

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

def GenerateClientPacketHandler(packetName):
    method_name = packetName + "Handler"
    return f"""
    public override void {method_name}()
    {{
        base.{method_name}();
        // TODO: Add client-specific handling here.
    }}
    """

def GeneratePCPacketHandler(packetName):
    method_name = packetName + "Handler"
    return f"""
    public override void {method_name}()
    {{
        // TODO: Implement the method.
    }}
    """

def ProcessPacketGenerate():
    with open(ymlFilePath, 'r') as file:
        ymlData = yaml.load(file, Loader=yaml.SafeLoader)
        
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
    # with open(clientPacketHandlerFilePath, 'w') as file:
    #     file.write(GenerateClientPacketHandler(packetList))
    
    # # Generate PCPacketHandler.cs
    # with open(pcPacketHandlerFilePath, 'w') as file:
    #     file.write(GeneratePCPacketHandler(packetList))


# Write file path here
packetTypeFilePath = 'CSharpGameServer/PacketType.cs'
protocolFilePath = 'CSharpGameServer/Protocol.cs'
packetHandlerFilePath = 'CSharpGameServer/PacketHandler.cs'
clientPacketHandlerFilePath = 'CSharpGameServer/Core/ClientManager.cs'
pcPacketHandlerFilePath = 'CSharpGameServer/PC/PCPacketHandler.cs'
ymlFilePath = 'PacketDefine.yml'

ProcessPacketGenerate()
print("Code generated successfully")
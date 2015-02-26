SET PWD=%CD%
cd proto
for /r %%a in (*.proto) do %PWD%/../ProtoGen/protogen -i:%%~na.proto -o:../../../Project/Assets/Scripts/Model/%%~na.cs -ns:com.pureland.proto
cd ..
@pause
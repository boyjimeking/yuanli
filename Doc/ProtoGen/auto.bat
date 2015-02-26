SET PWD=%CD%
cd proto
for /r %%a in (*.proto) do %PWD%/protogen -i:%%~na.proto -o:../../../Project/Assets/Scripts/Net/proto/%%~na.cs -ns:com.pureland.proto
cd ..
@pause
title "protoc��..."
@echo off
echo "��ʼprotoc"
rd /q /s generated
cd proto
mkdir generated
for %%a in (*.proto) do (protoc --java_out=./generated %%a)
move generated .. 
cd ..
@pause
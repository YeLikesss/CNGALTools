;ecx=DrcryptTable*
;edx=byte* key  (0x20字节由长度算得的key)

push ebp
mov ebp,esp
push ecx
cmp dword ptr ds:[0x009E6DB4],0x0
push ebx
push esi
push edi
mov esi,edx
mov edi,ecx

jne TableInitEnd:
call TableInit
mov dword ptr ds:[0x009E6DB4], 0x1
TableInitEnd:

lea edx,dword ptr ds:[edi+0x8]
mov dword ptr ds:[edi],0xE   ;放置轮解密循环次数

xor ebx,ebx     ;循环次数
mov dword ptr ds:[edi+0x4],edx  ;放置key表地址
add esi,0x2

CopyKey:
movzx ecx,byte ptr ds:[esi+0x1]
movzx eax,byte ptr ds:[esi]
lea esi,dword ptr ds:[esi+0x4]
shl ecx,0x8
or ecx,eax
movzx eax,byte ptr ds:[esi-0x5]
shl ecx,0x8
or ecx,eax
movzx eax,byte ptr ds:[esi-0x6]
shl ecx,8
or ecx,eax
;ecx=4字节key
mov dword ptr ds:[edx+ebx*0x4],ecx
inc ebx
cmp ebx,0x8
jb CopyKey:

mov eax,dword ptr ds:[edi]      ;获取轮解密次数
xor ebx,ebx
sub eax,0xA

je Round10:
dec eax
sub eax,1
je Round12:
dec eax
sub eax,1
jne End:
mov ecx,dword ptr ds:[edx]  ;edx=keyTable256*

add edx,0x20
mov dword ptr ss:[ebp-0x4],edx      ;当前key生成位置

xor edi,edi
loop1:
mov ebx,dword ptr ds:[edx-0x4]
movzx eax,bl
movzx esi,byte ptr ds:[eax+0xA00BC0]
mov eax,ebx
shr eax,0x18
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,ebx
shr eax,0x10
movzx eax,al
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,ebx
shr eax,0x8
movzx eax,al
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,dword ptr ds:[edx-0x1C]
xor esi,dword ptr ds:[edi+0xA011C0]
add edi,0x4
xor esi,ecx
mov ecx,dword ptr ds:[edx-0x18]
xor eax,esi
mov dword ptr ds:[edx],esi      ;保存key1
xor ecx,eax
mov dword ptr ds:[edx+0x4],eax  ;保存key2

mov eax,dword ptr ss:[ebp-0x4]
mov dword ptr ds:[edx+0x8],ecx  ;保存key3
mov edx,dword ptr ds:[edx-0x14]
xor edx,ecx
mov dword ptr ds:[eax+0xC],edx  ;保存key4

mov eax,edx
shr eax,0x18
movzx ecx,byte ptr ds:[eax+0xA00BC0]
mov eax,edx
shr eax,0x10
movzx eax,al
shl ecx,0x8

movzx eax,byte ptr ds:[eax+0xA00BC0]
xor ecx,eax

mov eax,edx
shr eax,8
movzx eax,al
shl ecx,8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor ecx,eax
movzx eax,dl
mov edx,dword ptr ss:[ebp-0x4]      ;当前key生成的位置
shl ecx,8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor ecx,eax
mov eax,dword ptr ds:[edx-0xC]
xor ecx,dword ptr ds:[edx-0x10]
xor eax,ecx
mov dword ptr ds:[edx+0x10],ecx  ;保存key5
mov ecx,dword ptr ds:[edx-0x8]
xor ecx,eax
mov dword ptr ds:[edx+0x14],eax  ;保存key6
mov dword ptr ds:[edx+0x18],ecx     ;保存key7
xor ecx,ebx
mov dword ptr ds:[edx+0x1C],ecx  ;保存key8

add edx,0x20       ;key生成位置自增
mov dword ptr ss:[ebp-0x4],edx  ;保存新的key生成指针
mov ecx,esi
cmp edi,0x1C
jb loop1:

End:
pop edi
pop esi
xor eax,eax
pop ebx
mov esp,ebp
pop ebp
ret


Round10:
mov ecx,dword ptr ds:[edx]      ;edx=keyTable256*
lea edi,dword ptr ds:[edx+0x10]

LoopR10_1:
mov edx,dword ptr ds:[edi-0x4]
movzx eax,dl
movzx esi,byte ptr ds:[eax+0xA00BC0]
mov eax,edx
shr eax,0x18
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,edx
shr eax,0x10
movzx eax,al
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,edx
shr eax,0x8
movzx eax,al
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,dword ptr ds:[edi-0xC]
xor esi,dword ptr ds:[ebx+0xA011C0]
add ebx,0x4
xor esi,ecx
mov ecx,dword ptr ds:[edi-0x8]
xor eax,esi
mov dword ptr ds:[edi],esi
xor ecx,eax
mov dword ptr ds:[edi+0x4],eax
mov dword ptr ds:[edi+0x8],ecx
lea edi,dword ptr ds:[edi+0x10]
xor ecx,edx
mov dword ptr ds:[edi-0x4],ecx
mov ecx,esi
cmp ebx,0x28
jb LoopR10_1:
jmp End:


Round12:
mov ecx,dword ptr ds:[edx]      ;edx=KeyTable256*
lea edi,dword ptr ds:[edx+0x18]

LoopR12_1:
mov edx,dword ptr ds:[edi-0x4]
movzx eax,dl
movzx esi,byte ptr ds:[eax+0xA00BC0]
mov eax,edx
shr eax,0x18
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,edx
shr eax,0x10
movzx eax,al
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,edx
shr eax,0x8
movzx eax,al
shl esi,0x8
movzx eax,byte ptr ds:[eax+0xA00BC0]
xor esi,eax
mov eax,dword ptr ds:[edi-0x14]
xor esi,dword ptr ds:[ebx+0xA011C0]
lea ebx,dword ptr ds:[ebx+0x4]
xor esi,ecx
mov ecx,dword ptr ds:[edi-0x10]
xor eax,esi
mov dword ptr ds:[edi],esi
xor ecx,eax
mov dword ptr ds:[edi+0x4],eax
mov eax,dword ptr ds:[edi-0xC]
lea edi,dword ptr ds:[edi+0x18]
xor eax,ecx
mov dword ptr ds:[edi-0x10],ecx
mov ecx,dword ptr ds:[edi-0x20]
xor ecx,eax
mov dword ptr ds:[edi-0xC],eax
mov dword ptr ds:[edi-0x8],ecx
xor ecx,edx
mov dword ptr ds:[edi-0x4],ecx
mov ecx,esi
cmp ebx,0x20
jb LoopR12_1:
jmp End:

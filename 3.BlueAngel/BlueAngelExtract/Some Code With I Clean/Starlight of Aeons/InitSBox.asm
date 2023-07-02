push ebp
mov ebp,esp
lea esp,dword ptr ss:[esp-0x808]
mov eax,dword ptr ds:[0x401A0C]
xor eax,ebp
mov dword ptr ss:[ebp-0x4],eax
push ebx
push esi
xor ebx,ebx
xor esi,esi
inc ebx
push edi

mov dword ptr ss:[ebp-0x808],ebx
mov edx,ebx
mov edi,0x100
Loop1:
mov eax,edx
mov dword ptr ss:[ebp+esi*0x4-0x804],edx
and al,0x80
mov dword ptr ss:[ebp+edx*0x4-0x404],esi
movzx ecx,al
lea eax,dword ptr ds:[edx+edx]
neg ecx
sbb ecx,ecx
and ecx,0x1B
xor ecx,eax
xor ecx,edx
inc esi
movzx edx,cl
cmp esi,edi
jl Loop1:

mov esi,ebx
mov edx,stars.0xA011C0
Loop2:
mov eax,esi
mov dword ptr ds:[edx],esi
and al,0x80
movzx ecx,al
lea eax,dword ptr ds:[esi+esi]
neg ecx
sbb ecx,ecx
add edx,0x4
and ecx,1B
xor ecx,eax
movzx esi,cl
cmp edx,stars.0xA011E8
jl Loop2: 

mov byte ptr ds:[0xA00BC0],0x63
mov byte ptr ds:[0xA01123],0x0

Loop3:
mov ecx,dword ptr ss:[ebp+ebx*0x4-0x404]
lea eax,dword ptr ss:[ebp-0x408]
shl ecx,0x2
sub eax,ecx
mov ebx,dword ptr ds:[eax]
mov ecx,ebx
sar ecx,0x7
lea eax,dword ptr ds:[ebx+ebx]
or ecx,eax
movzx eax,cl
xor ebx,eax
mov ecx,eax
shr ecx,0x7
add eax,eax
or ecx,eax
movzx eax,cl
xor ebx,eax
mov ecx,eax
shr ecx,0x7
lea eax,dword ptr ds:[eax+eax]
or ecx,eax
movzx edx,cl
mov ecx,edx
shr ecx,0x7
lea eax,dword ptr ds:[edx+edx]
or ecx,eax
xor ecx,0x63
movzx eax,cl
xor eax,edx
xor ebx,eax
mov eax,dword ptr ss:[ebp-0x808]
mov byte ptr ds:[eax+0xA00BC0],bl
mov byte ptr ds:[ebx+0xA010C0],al
mov ebx,eax
inc ebx
mov dword ptr ss:[ebp-0x808],ebx
cmp ebx,edi
jl Loop3:

sub esi,esi
mov edi,0xFF

Loop4:
movzx edx,byte ptr ds:[esi+0xA00BC0]
mov eax,edx
and al,0x80
movzx ecx,al
neg ecx
lea eax,dword ptr ds:[edx+edx]
sbb ecx,ecx
and ecx,0x1B
xor ecx,eax
movzx eax,cl
mov ecx,eax
xor ecx,edx
shl ecx,0x8
xor ecx,edx
shl ecx,0x8
xor ecx,edx
shl ecx,0x8
xor ecx,eax
mov dword ptr ds:[esi*4+0xA02650],ecx
rol ecx,0x8
mov dword ptr ds:[esi*4+0xA02250],ecx
rol ecx,0x8
mov dword ptr ds:[esi*4+0xA01650],ecx
rol ecx,0x8
mov dword ptr ds:[esi*4+0xA01250],ecx
movzx ecx,byte ptr ds:[esi+0xA010C0]
test ecx,ecx
je label1:

mov eax,dword ptr ss:[ebp+ecx*4-404]
add eax,dword ptr ss:[ebp-0x3CC]
cdq 
idiv edi
mov eax,dword ptr ss:[ebp+edx*0x4-0x804]
mov dword ptr ss:[ebp-0x808],eax
test ecx,ecx
je label2:

mov eax,dword ptr ss:[ebp+ecx*0x4-0x404]
add eax,dword ptr ss:[ebp-0x3E0]
cdq 
idiv edi
mov ebx,dword ptr ss:[ebp+edx*0x4-0x804]
test ecx,ecx
je label3:

mov eax,dword ptr ss:[ebp+ecx*0x4-0x404]
add eax,dword ptr ss:[ebp-0x3D0]
cdq 
idiv edi
mov edi,dword ptr ss:[ebp+edx*0x4-0x804]
test ecx,ecx
je label4:
mov eax,dword ptr ss:[ebp+ecx*4-404]
mov ecx,0xFF
add eax,dword ptr ss:[ebp-0x3D8]
cdq 
idiv ecx
mov eax,dword ptr ss:[ebp+edx*0x4-0x804]
jmp label5:

label1:
and dword ptr ss:[ebp-0x808],0x0

label2:
sub ebx,ebx

label3:
sub edi,edi

label4:
xor eax,eax

label5:
shl eax,8
xor eax,edi
mov edi,0xFF
shl eax,8
xor eax,ebx
shl eax,8
xor eax,dword ptr ss:[ebp-0x808]
mov dword ptr ds:[esi*4+0xA01E50],eax
rol eax,8
mov dword ptr ds:[esi*4+0xA01A50],eax
rol eax,8
mov dword ptr ds:[esi*4+0xA02A50],eax
rol eax,8
mov dword ptr ds:[esi*4+0xA00CC0],eax
inc esi
cmp esi,0x100
jl Loop4:

mov ecx,dword ptr ss:[ebp-4]
pop edi
pop esi
xor ecx,ebp
pop ebx
call stars.0x713318
mov esp,ebp
pop ebp
ret
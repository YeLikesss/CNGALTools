push ebp
mov ebp,esp
push ecx
push ecx
push ebx
push esi
push edi
mov edi,ecx
xor ebx,ebx
mov eax,edx
mov dword ptr ss:[ebp-0x4],eax
mov esi,dword ptr ds:[edi+0xC8]
cmp dword ptr ss:[ebp+8],ebx
jbe End:

loopInit:
mov al,byte ptr ds:[ebx+eax]
xor byte ptr ds:[esi+edi],al
inc esi
cmp esi,dword ptr ds:[edi+0xCC]
jl label2:
mov ecx,edi   ; hashTablePointer >= hashTable.MaxSeedLength
Call CreateHash
xor esi,esi
label2:
mov eax,dword ptr ss:[ebp-0x4]
inc ebx  ;seedPointer++
cmp ebx,dword ptr ss:[ebp+0x8]
jb loopInit:

End:
mov dword ptr ds:[edi+0xC8],esi
xor eax,eax
pop edi
pop esi
inc eax
pop ebx

mov esp,ebp
pop ebp
ret 





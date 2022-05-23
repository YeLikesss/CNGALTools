push ebp
mov ebp,esp
sub esp,0x1D4
movaps xmm0,xmmword ptr ds:[0xB69690]                     
movups xmmword ptr ss:[ebp-0x150],xmm0                 
push ebx
movaps xmm0,xmmword ptr ds:[0xB69710]   
sub ebx,ebx
movups xmmword ptr ss:[ebp-0x140],xmm0  
push esi
movaps xmm0,xmmword ptr ds:[0xB696B0]   
mov esi,ecx
movups xmmword ptr ss:[ebp-0x130],xmm0  
mov ecx,0x80008081
mov dword ptr ss:[ebp-0x1C0],esi
movaps xmm0,xmmword ptr ds:[0xB69650]   
movups xmmword ptr ss:[ebp-0x120],xmm0  
push edi
movaps xmm0,xmmword ptr ds:[0xB696F0]   
mov edi,0x80000000
movups xmmword ptr ss:[ebp-0x110],xmm0  
mov dword ptr ss:[ebp-0xF0],0x1         
movaps xmm0,xmmword ptr ds:[0xB69720]   
movups xmmword ptr ss:[ebp-0x100],xmm0  
lea edx,dword ptr ds:[edi+0x1]
mov dword ptr ss:[ebp-0xEC],ebx         
movaps xmm0,xmmword ptr ds:[0xB696E0]   
lea eax,dword ptr ds:[edi+0xA]
movups xmmword ptr ss:[ebp-0x1B0],xmm0  
mov dword ptr ss:[ebp-0xE8],0x8082      
movaps xmm0,xmmword ptr ds:[0xB696D0]   
movups xmmword ptr ss:[ebp-0x1A0],xmm0  
mov dword ptr ss:[ebp-0xE4],ebx         
movaps xmm0,xmmword ptr ds:[0xB69600]   
movups xmmword ptr ss:[ebp-0x190],xmm0  
mov dword ptr ss:[ebp-0xE0],0x808A      
movaps xmm0,xmmword ptr ds:[0xB696A0]   
movups xmmword ptr ss:[ebp-0x180],xmm0  
mov dword ptr ss:[ebp-0xDC],edi         
movaps xmm0,xmmword ptr ds:[0xB696C0]   
movups xmmword ptr ss:[ebp-0x170],xmm0  
mov dword ptr ss:[ebp-0xD8],0x80008000  
movaps xmm0,xmmword ptr ds:[0xB69540]   
mov dword ptr ss:[ebp-0xD4],edi         
mov dword ptr ss:[ebp-0xD0],0x808B      
mov dword ptr ss:[ebp-0xCC],ebx         
mov dword ptr ss:[ebp-0xC8],edx         
mov dword ptr ss:[ebp-0xC4],ebx         
mov dword ptr ss:[ebp-0xC0],ecx         
mov dword ptr ss:[ebp-0xBC],edi         
mov dword ptr ss:[ebp-0xB8],0x8009      
mov dword ptr ss:[ebp-0xB4],edi         
mov dword ptr ss:[ebp-0xB0],0x8A        
mov dword ptr ss:[ebp-0xAC],ebx         
mov dword ptr ss:[ebp-0xA8],0x88        
mov dword ptr ss:[ebp-0xA4],ebx         
mov dword ptr ss:[ebp-0xA0],0x80008009  
mov dword ptr ss:[ebp-0x9C],ebx         
mov dword ptr ss:[ebp-0x98],eax         
mov dword ptr ss:[ebp-0x94],ebx         
mov dword ptr ss:[ebp-0x90],0x8000808B  
mov dword ptr ss:[ebp-0x8C],ebx         
mov dword ptr ss:[ebp-0x88],0x8B        
mov dword ptr ss:[ebp-0x84],edi         
mov dword ptr ss:[ebp-0x80],0x8089      
mov dword ptr ss:[ebp-0x7C],edi         
mov dword ptr ss:[ebp-0x78],0x8003      
mov dword ptr ss:[ebp-0x74],edi         
mov dword ptr ss:[ebp-0x70],0x8002      
mov dword ptr ss:[ebp-0x6C],edi         
mov dword ptr ss:[ebp-0x68],0x80        
mov dword ptr ss:[ebp-0x64],edi         
mov dword ptr ss:[ebp-0x60],0x800A      
mov dword ptr ss:[ebp-0x5C],ebx         
mov dword ptr ss:[ebp-0x58],eax         
mov dword ptr ss:[ebp-0x54],edi         
mov dword ptr ss:[ebp-0x50],ecx         
mov dword ptr ss:[ebp-0x4C],edi         
mov dword ptr ss:[ebp-0x48],0x8080      
mov dword ptr ss:[ebp-0x44],edi         
mov dword ptr ss:[ebp-0x40],edx         
mov dword ptr ss:[ebp-0x3C],ebx         
mov dword ptr ss:[ebp-0x38],0x80008008  
mov dword ptr ss:[ebp-0x34],edi         
movups xmmword ptr ss:[ebp-0x160],xmm0  
mov dword ptr ss:[ebp-0x1C8],ebx        ;loop7 count
lea eax,dword ptr ds:[esi+0x78]         ;Pointer
mov dword ptr ss:[ebp-0x1D0],0x5


;0x00BE32E0
loop7:
mov esi,0x5
mov edi,ebx       ;ebx = 0
mov edx,eax       ;eax = HashTable*


;0x00BE32FE
loop1:
mov ecx,dword ptr ds:[edx-0x78]
xor ecx,dword ptr ds:[edx-0x50]
mov eax,dword ptr ds:[edx-0x74]
xor eax,dword ptr ds:[edx-0x4C]
xor ecx,dword ptr ds:[edx-0x28]
xor eax,dword ptr ds:[edx-0x24]
xor ecx,dword ptr ds:[edx+0x28]
xor eax,dword ptr ds:[edx+0x2C]
xor ecx,dword ptr ds:[edx]
lea edx,dword ptr ds:[edx+0x8]
xor eax,dword ptr ds:[edx-0x4]
mov dword ptr ss:[ebp+edi*0x8-0x30],ecx
mov dword ptr ss:[ebp+edi*0x8-0x2C],eax
inc edi
cmp edi,esi
jl loop1:       ;0x00D048C1
mov ebx,dword ptr ss:[ebp-0x1C0]       ;HashTable*
mov edi,0x4
mov eax,0x5
mov dword ptr ss:[ebp-0x1B4],edi       ;edi=4
mov dword ptr ss:[ebp-0x1B8],eax       ;eax=5 

;0x00D04901
loop3:
lea eax,dword ptr ds:[edi-0x3]
cdq 
mov esi,0x5
idiv esi
mov ecx,dword ptr ss:[ebp+edx*0x8-0x30]
mov eax,dword ptr ss:[ebp+edx*0x8-0x2C]
mov edx,eax
shld eax,ecx,0x1
shr edx,0x1F
add ecx,ecx
or edx,ecx
sub ecx,ecx
or ecx,eax
mov dword ptr ss:[ebp-0x1C4],edx
mov eax,edi
mov dword ptr ss:[ebp-0x1BC],ecx
cdq 
idiv esi
mov ecx,dword ptr ss:[ebp-0x1C4]
mov eax,dword ptr ss:[ebp-0x1BC]
xor ecx,dword ptr ss:[ebp+edx*0x8-0x30]
xor eax,dword ptr ss:[ebp+edx*0x8-0x2C]
mov edx,esi
mov dword ptr ss:[ebp-0x1BC],eax
mov eax,ebx     ;HashTable*
mov edi,dword ptr ss:[ebp-0x1BC]

;0x00C8A00F
loop2:
xor dword ptr ds:[eax],ecx
lea eax,dword ptr ds:[eax+0x28]
xor dword ptr ds:[eax-0x24],edi
sub edx,0x1
jne loop2:      ;0x00CAD44D
mov edi,dword ptr ss:[ebp-0x1B4]
add ebx,0x8
inc edi
sub dword ptr ss:[ebp-0x1B8],0x1
mov dword ptr ss:[ebp-0x1B4],edi
jne loop3:      ;0x00CF198E
mov esi,dword ptr ss:[ebp-0x1C0]
mov dword ptr ss:[ebp-0x1BC],edx
mov edi,dword ptr ds:[esi+0x8]
mov eax,dword ptr ds:[esi+0xC]
mov dword ptr ss:[ebp-0x1B8],edi
mov dword ptr ss:[ebp-0x1B4],eax


;0x00CF19C3
loop4:      ;edx=loopCount
mov ebx,dword ptr ss:[ebp+edx-0x1B0]
mov ecx,0x40
sub ecx,dword ptr ss:[ebp+edx-0x150]
mov eax,dword ptr ds:[esi+ebx*0x8]
mov edx,dword ptr ss:[ebp-0x1B4]
mov dword ptr ss:[ebp-0x1C4],eax
mov eax,dword ptr ds:[esi+ebx*0x8+0x4]
mov dword ptr ss:[ebp-0x1CC],eax
mov eax,edi
call SubFunc1:
mov ecx,dword ptr ss:[ebp-0x1BC]
mov esi,eax
mov eax,dword ptr ss:[ebp-0x1B8]
mov edi,edx
mov edx,dword ptr ss:[ebp-0x1B4]
mov ecx,dword ptr ss:[ebp+ecx-0x150]
call SubFunc2:
or esi,eax
or edi,edx
mov eax,dword ptr ss:[ebp-0x1C0]
;loop+=4
mov edx,dword ptr ss:[ebp-0x1BC]
lea edx,dword ptr ds:[edx+0x4]
mov dword ptr ss:[ebp-0x1BC],edx

mov dword ptr ds:[eax+ebx*0x8],esi
mov esi,eax

mov eax,dword ptr ss:[ebp-0x1CC]
mov dword ptr ss:[ebp-0x1B4],eax

mov dword ptr ds:[esi+ebx*0x8+0x4],edi
mov edi,dword ptr ss:[ebp-0x1C4]
mov dword ptr ss:[ebp-0x1B8],edi
cmp edx,0x60
jl loop4:

mov eax,0x5
mov ebx,esi         ;ebx=HashTable*
mov dword ptr ss:[ebp-0x1B4],eax

;0x00CBDF68
loop6:
mov ecx,0xA
mov esi,ebx
mov dword ptr ss:[ebp-0x1B8],eax
lea edi,dword ptr ss:[ebp-0x30]
rep movsd 
mov edi,0x2


;0x00CBDF92
loop5:
lea eax,dword ptr ds:[edi-0x1]
cdq 

;0x0047CC83
mov ecx,0x5
idiv ecx
mov eax,edi
mov esi,dword ptr ss:[ebp+edx*0x8-0x30]
not esi
mov ecx,dword ptr ss:[ebp+edx*0x8-0x2C]
cdq 
not ecx
idiv dword ptr ss:[ebp-0x1D0]
and esi,dword ptr ss:[ebp+edx*0x8-0x30]
and ecx,dword ptr ss:[ebp+edx*0x8-0x2C]
xor dword ptr ds:[ebx],esi
xor dword ptr ds:[ebx+4],ecx
add ebx,0x8
inc edi
sub dword ptr ss:[ebp-0x1B8],0x1
jne loop5:

sub dword ptr ss:[ebp-0x1B4],0x1
mov eax,0x5
jne loop6:

;0x00C08DD4
mov ecx,dword ptr ss:[ebp-0x1C8]
mov esi,dword ptr ss:[ebp-0x1C0]
xor ebx,ebx
mov eax,dword ptr ss:[ebp+ecx*0x8-0xF0]
xor dword ptr ds:[esi],eax
mov eax,dword ptr ss:[ebp+ecx*0x8-0xEC]
xor dword ptr ds:[esi+0x4],eax
inc ecx
mov dword ptr ss:[ebp-0x1C8],ecx
lea eax,dword ptr ds:[esi+0x78]
cmp ecx,18
jl loop7:

pop edi
pop esi
pop ebx
mov esp,ebp
pop ebp
ret






;SubFunc1
cmp cl,40
jae labelsub1:
cmp cl,20
jae labelsub2:
shrd eax,edx,cl
shr edx,cl
ret 

labelsub2:
mov eax,edx
xor edx,edx
and cl,1F
shr eax,cl
ret 

labelsub1:
xor eax,eax
xor edx,edx
ret 

;SubFunc2:
cmp cl,40
jae labelsub21:
cmp cl,20
jae labelsub22:
shld edx,eax,cl
shl eax,cl
ret 

labelsub22:
mov edx,eax
xor eax,eax
and cl,1F
shl edx,cl
ret 

labelsub21:
xor eax,eax
xor edx,edx
ret 
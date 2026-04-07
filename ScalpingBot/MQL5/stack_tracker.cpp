// #include "stack_tracker.h"

// auto StackTracker::rpm(uintptr_t address, size_t readSize)
//     -> std::vector<char> {
//     size_t NumOfRead = 0;
//     std::vector<char> buffer(readSize);

//     if (ReadProcessMemory(this->targetProcess, (LPCVOID)address, buffer.data(),
//                           readSize, &NumOfRead) == false ||
//         NumOfRead != readSize) {
//         return {};
//     }
//     return buffer;
// }
// auto StackTracker::LookslikeValidEntry(cs_insn* insn, size_t count) -> bool {
//     if (insn == nullptr || count == 0) return false;

//     int threshold_score = 2;
//     int score = 0;

//     // 限制最多检查前几条指令
//     size_t check_limit = min(count, static_cast<size_t>(8));

//     for (size_t i = 0; i < check_limit; ++i) {
//         const cs_insn& inst = insn[i];

//         switch (inst.id) {
//             case X86_INS_PUSH:
//                 if (strcmp(inst.mnemonic, "push") == 0) score++;
//                 break;
//             case X86_INS_MOV:
//                 if (strstr(inst.op_str, "rbp") != nullptr ||
//                     strstr(inst.op_str, "rsp") != nullptr)
//                     score++;
//                 break;
//             case X86_INS_SUB:
//             case X86_INS_ADD:
//                 if (strstr(inst.op_str, "rsp") != nullptr) score++;
//                 break;
//             case X86_INS_CALL:
//                 score += 1;
//                 break;
//             case X86_INS_LEA:
//                 if (strstr(inst.op_str, "rip") != nullptr) score++;
//                 break;
//             case X86_INS_TEST:
//             case X86_INS_CMP:
//             case X86_INS_JE:
//             case X86_INS_JNE:
//             case X86_INS_JMP:
//                 score++;
//                 break;
//             case X86_INS_NOP:
//                 break;  // 忽略
//             default:
//                 if (score == 0) score -= 1;  // 杂指令降低一点分数
//                 break;
//         }

//         if (score >= threshold_score) {
//             return true;
//         }
//     }
//     return score >= threshold_score;
// }
// auto StackTracker::TryFindValidDisasm(uint64_t baseAddr, size_t maxOffset)
//     -> bool {
//     for (size_t i = 0; i < maxOffset; ++i) {
//         auto buf = this->rpm(baseAddr + i, this->trackSize);
//         if (buf.size() != this->trackSize) continue;
//         cs_insn* testInsn = nullptr;
//         this->disasmCount = cs_disasm(this->capstoneHandle,
//                              reinterpret_cast<const uint8_t*>(buf.data()),
//                              this->trackSize, baseAddr + i, 0, &testInsn);
//         // this->PrintAsm(testInsn);
//         if (this->disasmCount > 0) {
//             this->insn = testInsn;
//         }
//         if (this->disasmCount > 0 && LookslikeValidEntry(testInsn, this->disasmCount)) {
//             this->baseAddr += i;
//             if (this->insn != nullptr) {
//                 cs_free(this->insn, this->disasmCount);
//             }
//             for (size_t j = 0; j < this->disasmCount; ++j) {
//                 // this->PrintAsm(&this->insn[j]);

//                 this->insList.push_back(
//                     std::make_shared<cs_insn>(this->insn[j]));
//             }
//             this->SuccessReadedBuffer = buf;
//             this->readSuccess = true;
//             return true;
//         }
//     }
//     return false;
// }
// StackTracker::StackTracker(HANDLE hProcess, uint64_t StartAddress,
//                            size_t trackSize, bool isX32) {
//     this->isWow64 = isX32;
//     this->targetProcess = hProcess;
//     this->baseAddr = StartAddress;
//     this->trackSize = trackSize;
//     if (cs_open(CS_ARCH_X86, this->isWow64 ? CS_MODE_32 : CS_MODE_64,
//                 &capstoneHandle) != CS_ERR_OK) {
//         __debugbreak();
//     }
//     cs_option(capstoneHandle, CS_OPT_DETAIL, CS_OPT_ON);
//     cs_option(capstoneHandle, CS_OPT_SKIPDATA, CS_OPT_ON);
//     /*
//     do {
//         // 1.读取
//         auto bufferArrays = this->rpm(StartAddress, trackSize);
//         if (bufferArrays.size() != trackSize) {
//             break;
//         }
//         // 2. 反过来
//         std::reverse(bufferArrays.begin(), bufferArrays.end());
//         // 3. 这里就是向上的了.指令是对的上的
//         disasmCount =
//             cs_disasm(capstoneHandle,
//                       reinterpret_cast<const uint8_t*>(bufferArrays.data()),
//                       trackSize, StartAddress, 0, &insn);
//         if (disasmCount == 0) {
//             break;
//         }
//         // 4. 再反过来
//         for (size_t index = disasmCount; index > 0; index--) {
//             const auto code = insn[index];
//             this->PrintAsm(&code);
//             this->insList.push_back(std::make_shared<cs_insn>(code));
//         }
//         this->readSuccess = true;
//     } while (false);
//     */
// }

// auto StackTracker::getNextIns() -> std::shared_ptr<cs_insn> {
//     if (this->ins_ip >= this->insList.size()) {
//         return nullptr;
//     }
//     const auto result = this->insList[this->ins_ip];
//     this->ins_ip++;
//     this->ins_ip_address = result->address;
//     return result;
// }
// StackTracker::~StackTracker() {
//     if (insn) {
//         //cs_free(insn, disasmCount);
//         cs_close(&capstoneHandle);
//     }
// }
// template <typename T, typename B>
// auto StackTracker::matchCode(
//     T match_fn, B process_fn, std::optional<uint32_t> num_operands,
//     std::vector<std::optional<x86_op_type>> operand_types) -> bool {
//     while (auto instruction = getNextIns()) {
//         if (&process_fn != nullptr) {
//             process_fn(instruction.get());
//         }
//         if (num_operands) {
//             if (instruction->detail->x86.op_count != *num_operands) continue;
//             bool operand_type_mismatch = false;
//             for (uint32_t i = 0; i < *num_operands; i++) {
//                 auto& target_type = operand_types[i];
//                 if (target_type &&
//                     target_type != instruction->detail->x86.operands[i].type) {
//                     operand_type_mismatch = true;
//                     break;
//                 }
//             }
//             if (operand_type_mismatch) continue;
//         }
//         if (match_fn(instruction.get())) return true;
//     }
//     return false;
// }

// inline auto StackTracker::is_call(cs_insn* ins) -> bool {
//     return ins->id == X86_INS_CALL;
// }
// auto StackTracker::PrintAsm() -> void {
//     for (size_t j = 0; j < this->disasmCount; ++j) {
//         for (int x = 0; x < this->insn[j].size; x++) {
//             printf("%02X ", this->insn[j].bytes[x]);
//         }
//         printf("0x%llx :\t\t%s\t%s\t\n", this->insn[j].address,
//                this->insn[j].mnemonic, this->insn[j].op_str);

//     }

// }
// auto StackTracker::CalcNextJmpAddress() -> std::pair<bool, uint64_t> {
//     if (this->readSuccess == false) {
//         return {false, 0};
//     }
//     this->feature = _features::kNonCallOnly;

//     uint64_t callAddress = 0;
//     auto isMatchCall = matchCode(
//         [&](cs_insn* instruction) {
//             if (instruction->id != X86_INS_CALL) {
//                 if (instruction->id == X86_INS_SYSCALL) {
//                     this->feature = _features::kSyscall;
//                 }
//                 return false;
//             }
//             if (instruction->detail->x86.op_count != 1) {
//                 return false;
//             }
//             const cs_x86_op& operand = instruction->detail->x86.operands[0];
//             if (operand.type == X86_OP_IMM) {
//                 callAddress =
//                     instruction->address + instruction->size + operand.imm;
//                 return true;
//             } else if (operand.type == X86_OP_MEM) {
//                 const x86_op_mem& mem = operand.mem;
//                 // 我们只处理可以静态计算的 RIP 相对寻址
//                 if (mem.base == X86_REG_RIP) {
//                     uint64_t pointerAddress =
//                         instruction->address + instruction->size + mem.disp;
//                     size_t pointerSize = this->isWow64 ? 4 : 8;
//                     std::vector<char> pointerBuffer =
//                         this->rpm(pointerAddress, pointerSize);
//                     if (pointerBuffer.empty()) {
//                         std::cerr << "Failed to read pointer at 0x" << std::hex
//                                   << pointerAddress << std::endl;
//                         return false;
//                     }
//                     if (pointerSize == 8) {
//                         callAddress =
//                             *reinterpret_cast<uint64_t*>(pointerBuffer.data());
//                     } else {  // 32位
//                         callAddress =
//                             *reinterpret_cast<uint32_t*>(pointerBuffer.data());
//                     }

//                     // std::cout << "Found RIP-relative call at 0x" << std::hex
//                     // << instruction->address
//                     //     << ". Pointer at 0x" << pointerAddress
//                     //     << ". Final Target: 0x" << callAddress << std::endl;
//                     return true;
//                 }
//                 // std::cout << "Skipping non-RIP-relative memory call at 0x" <<
//                 // std::hex << instruction->address << std::endl;
//                 this->feature = _features::kCallRip;
//                 return false;
//             } else if (operand.type == X86_OP_REG) {
//                 this->feature = _features::kCallReg;
//                 return false;
//             }
//             return false;
//         },
//         [&](cs_insn* instruction) {}, {}, {});
//     return {isMatchCall, callAddress};
// }

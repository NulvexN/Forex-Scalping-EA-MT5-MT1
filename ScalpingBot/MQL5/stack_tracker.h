// #pragma once
// #include "head.h"
// enum class _features { kNone, kNonCallOnly, kCallRip, kCallReg, kSyscall };
// class StackTracker {
//    private:
//     bool readSuccess;
//     bool isWow64;
//     HANDLE targetProcess;
//     std::vector<std::shared_ptr<cs_insn>> insList;
//     csh capstoneHandle;
//     uint64_t ins_ip, ins_ip_address, baseAddr, trackSize;
//     auto getNextIns() -> std::shared_ptr<cs_insn>;
//     auto LookslikeValidEntry(cs_insn* insn, size_t count) -> bool;
//     inline auto is_call(cs_insn* ins) -> bool;

//     template <typename T, typename B>
//     auto matchCode(T match_fn, B process_fn,
//                    std::optional<uint32_t> num_operands,
//                    std::vector<std::optional<x86_op_type>> operand_types)
//         -> bool;
//     auto rpm(uintptr_t address, size_t readSize) -> std::vector<char>;

//    public:
//     cs_insn* insn = nullptr;
//     size_t disasmCount = 0;
//     std::vector<char> SuccessReadedBuffer;
//     _features feature;
//     StackTracker(HANDLE hProcess, uint64_t StartAddress, size_t trackSize,
//                  bool isX32);
//     ~StackTracker();
//     auto PrintAsm() -> void;
//     auto CalcNextJmpAddress() -> std::pair<bool, uint64_t>;
//     auto TryFindValidDisasm(uint64_t baseAddr, size_t maxOffset) -> bool;
// };

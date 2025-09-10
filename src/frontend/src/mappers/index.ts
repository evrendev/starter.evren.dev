import type { User, BasicUser } from "@/models/user";
import { useDateFormat } from "@vueuse/core";

const Mapper = {
  toUser(value: User | undefined): BasicUser {
    if (!value) {
      return {} as BasicUser;
    }

    return {
      id: value.id,
      gender: value.gender,
      email: value.email,
      language: value.language,
      firstName: value.firstName,
      lastName: value.lastName,
      fullName: value.fullName,
      initial: value.initial,
      phoneNumber: value.phoneNumber,
      birthday: useDateFormat(value.birthday, "YYYY-MM-DD").value,
      placeOfBirth: value.placeOfBirth,
      twoFactorEnabled: value.twoFactorEnabled,
    };
  },
};

export default Mapper;

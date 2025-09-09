import type { User, BasicUser } from "@/models/user";

const Mapper = {
  toUser(value: User): BasicUser {
    return {
      id: value.id,
      gender: value.gender,
      email: value.email,
      language: value.language,
      firstName: value.firstName,
      lastName: value.lastName,
      initial: value.initial,
      phoneNumber: value.phoneNumber,
      birthday: value.birthday,
      placeOfBirth: value.placeOfBirth,
    };
  },
};

export default Mapper;

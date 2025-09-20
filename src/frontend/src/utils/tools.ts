import { FileUploadRequest } from "@/models/app";

const convertToUploadRequest = async (
  uploadedFile: File | File[] | undefined,
) => {
  if (!uploadedFile) return undefined;

  const file: File = Array.isArray(uploadedFile)
    ? uploadedFile[0]
    : uploadedFile;

  const base64Data = await toBase64(file);
  const extension = file.name.split(".").pop() || "";

  const uploadRequest: FileUploadRequest = {
    name: file.name,
    extension: `.${extension}`,
    data: base64Data,
  };

  return uploadRequest;
};

const toBase64 = (file: File): Promise<string> =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      const result = (reader.result as string).split(",")[1];
      resolve(result);
    };
    reader.onerror = (error) => reject(error);
  });

export { convertToUploadRequest };

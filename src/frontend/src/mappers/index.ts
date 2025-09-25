import { Course } from "@/models/course";
import { Lesson } from "@/models/lesson";

const BASE_URL = import.meta.env.VITE_APP_IMAGE_BASE_URL;

const Mapper = {
  async toCourse(value: Course | undefined): Promise<Course> {
    if (!value) {
      return {} as Course;
    }

    const imageModel: File[] | File | undefined = [];

    if (typeof value.image === "string" && value.image) {
      const file = await urlToFile(
        `${BASE_URL}/${value.image}`,
        getFileNameFromPath(value.image),
      );

      if (file) {
        imageModel.push(file);
      }
    }

    return {
      id: value.id,
      categoryId: value.categoryId,
      title: value.title,
      description: value.description,
      introduction: value.introduction,
      tags: value.tags,
      published: value.published,
      image: imageModel,
      previewVideoUrl: value.previewVideoUrl,
    };
  },

  async toLesson(value: Lesson | undefined): Promise<Lesson> {
    if (!value) {
      return {} as Lesson;
    }

    const imageModel: File[] | File | undefined = [];

    if (typeof value.image === "string" && value.image) {
      const file = await urlToFile(
        `${BASE_URL}/${value.image}`,
        getFileNameFromPath(value.image),
      );

      if (file) {
        imageModel.push(file);
      }
    }

    return {
      id: value.id,
      chapterId: value.chapterId,
      title: value.title,
      description: value.description,
      content: value.content,
      notes: value.notes,
      image: imageModel,
    };
  },
};

async function urlToFile(url: string, filename: string): Promise<File | null> {
  try {
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error(`Network response was not ok: ${response.statusText}`);
    }

    const blob = await response.blob();

    const file = new File([blob], filename, { type: blob.type });

    return file;
  } catch (error) {
    console.error("Error converting URL to File:", error);
    return null;
  }
}

function getFileNameFromPath(path: string): string {
  return path.split("/").pop() || "file";
}

export default Mapper;

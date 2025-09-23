export interface Course {
  id: string;
  categoryId: string;
  categoryName?: string;
  title: string;
  introduction: string | null;
  description: string | null;
  tags: string[];
  image: File | File[] | undefined;
  published: boolean;
  previewVideoUrl: string | null;
}

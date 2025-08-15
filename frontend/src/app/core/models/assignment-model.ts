export type AssignmentModel = {
  id: string | null | undefined;
  title: string;
  userName: string;
  description: string;
  truncatedDescription?: string;
  dueDate: Date;
  priority: number;
  priorityDescription?: string | null | undefined;
  status: number;
};

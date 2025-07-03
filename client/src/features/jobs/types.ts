import { Author } from "../users/types";

export interface CreateJobOfferRequest {
  title: string;
  description: string;
  budget: number;
  area: string;
  workerId: string;
}

export interface JobOfferResponse {
  id: string;
  title: string;
  description: string;
  budget: number;
  area: string;
  createdAt: string;
  author: Author;
}

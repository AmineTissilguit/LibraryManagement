import type {
  Book,
  Member,
  CreateBookRequest,
  CreateMemberRequest,
  BorrowBookRequest,
} from "../types";

const API_BASE_URL = "http://localhost:5105/api";

class ApiError extends Error {
  status: number;
  errors?: Record<string, string[]>;

  constructor(
    status: number,
    message: string,
    errors?: Record<string, string[]>
  ) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.errors = errors;
  }
}

async function apiRequest<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;

  const config: RequestInit = {
    headers: {
      "Content-Type": "application/json",
      ...options?.headers,
    },
    ...options,
  };

  const response = await fetch(url, config);

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));

    // Handle validation errors (400 status with errors object)
    if (response.status === 400 && errorData.errors) {
      const errorMessages = Object.entries(errorData.errors)
        .map(
          ([field, messages]) =>
            `${field}: ${(messages as string[]).join(", ")}`
        )
        .join("; ");

      throw new ApiError(response.status, errorMessages, errorData.errors);
    }

    throw new ApiError(
      response.status,
      errorData.title || errorData.message || "An error occurred"
    );
  }

  return response.json();
}

// Books API
export const booksApi = {
  getAll: (): Promise<Book[]> => apiRequest("/books"),

  getById: (id: number): Promise<Book> => apiRequest(`/books/${id}`),

  search: (searchTerm: string): Promise<Book[]> =>
    apiRequest(`/books/search?searchTerm=${encodeURIComponent(searchTerm)}`),

  create: (book: CreateBookRequest): Promise<Book> =>
    apiRequest("/books", {
      method: "POST",
      body: JSON.stringify(book),
    }),
};

// Members API
export const membersApi = {
  getById: (id: number): Promise<Member> => apiRequest(`/members/${id}`),

  create: (member: CreateMemberRequest): Promise<Member> =>
    apiRequest("/members", {
      method: "POST",
      body: JSON.stringify(member),
    }),
};

// Borrowing Transactions API
export const transactionsApi = {
  borrow: (
    request: BorrowBookRequest
  ): Promise<{ transactionId: number; message: string }> =>
    apiRequest("/borrowingtransactions/borrow", {
      method: "POST",
      body: JSON.stringify(request),
    }),

  return: (transactionId: number): Promise<{ message: string }> =>
    apiRequest(`/borrowingtransactions/return/${transactionId}`, {
      method: "POST",
    }),
};

export interface Fund { id: number; name: string; amountPerStudent: number }
export interface Student { id: number; firstName: string; lastName: string }
export interface Balance { studentId: number; studentName: string; fundId: number; fundName: string; owed: number; paid: number; balance: number; status: 'Paid' | 'Partial' | 'Unpaid' }
export interface Summary { fundId: number; fundName: string; expected: number; collected: number; outstanding: number }
export interface Payment { id: number; amount: number; method: string; paidAt: string; recordedBy: string; student: Student; fund: { name: string } }
export interface Session { token: string; role: string; username: string }

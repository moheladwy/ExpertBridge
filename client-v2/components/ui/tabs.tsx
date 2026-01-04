"use client";

import * as React from "react";
import { cn } from "@/lib/utils";

interface TabsContextValue {
	value: string;
	onChange: (value: string) => void;
}

const TabsContext = React.createContext<TabsContextValue | null>(null);
function useTabs() {
	const context = React.useContext(TabsContext);
	if (!context) {
		throw new Error("Tabs components must be used within a Tabs provider");
	}
	return context;
}

interface TabsProps {
	value: string;
	onValueChange: (value: string) => void;
	children: React.ReactNode;
	className?: string;
}

/**
 * Tabs container component.
 */
function Tabs({ value, onValueChange, children, className }: TabsProps) {
	return (
		<TabsContext.Provider value={{ value, onChange: onValueChange }}>
			<div className={cn("w-full", className)}>{children}</div>
		</TabsContext.Provider>
	);
}

interface TabsListProps {
	children: React.ReactNode;
	className?: string;
}

/**
 * Tabs list container for tab triggers.
 */
function TabsList({ children, className }: TabsListProps) {
	return (
		<div
			role="tablist"
			className={cn(
				"inline-flex h-10 items-center justify-start gap-1 rounded-lg bg-muted p-1",
				className
			)}
		>
			{children}
		</div>
	);
}

interface TabsTriggerProps {
	value: string;
	children: React.ReactNode;
	className?: string;
	disabled?: boolean;
}

/**
 * Individual tab trigger button.
 */
function TabsTrigger({
	value,
	children,
	className,
	disabled = false,
}: TabsTriggerProps) {
	const { value: selectedValue, onChange } = useTabs();
	const isSelected = selectedValue === value;

	return (
		<button
			role="tab"
			type="button"
			aria-selected={isSelected}
			disabled={disabled}
			onClick={() => { onChange(value); }}
			className={cn(
				"inline-flex items-center justify-center whitespace-nowrap rounded-md px-3 py-1.5 text-sm font-medium ring-offset-background transition-all",
				"focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
				"disabled:pointer-events-none disabled:opacity-50",
				isSelected
					? "bg-background text-foreground shadow-sm"
					: "text-muted-foreground hover:text-foreground",
				className
			)}
		>
			{children}
		</button>
	);
}

interface TabsContentProps {
	value: string;
	children: React.ReactNode;
	className?: string;
}

/**
 * Tab content panel.
 */
function TabsContent({ value, children, className }: TabsContentProps) {
	const { value: selectedValue } = useTabs();
	const isSelected = selectedValue === value;

	if (!isSelected) return null;

	return (
		<div
			role="tabpanel"
			className={cn(
				"mt-2 ring-offset-background",
				"focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
				className
			)}
		>
			{children}
		</div>
	);
}

export { Tabs, TabsList, TabsTrigger, TabsContent };

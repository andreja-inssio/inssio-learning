def calculate_average(numbers):
    # Start typing here and watch Copilot suggest the function body
    if not numbers:
        return 0
    total = sum(numbers)
    count = len(numbers)
    average = total / count
    return average

# function to reverse a string
def reverse_string(s):
    return s[::-1]
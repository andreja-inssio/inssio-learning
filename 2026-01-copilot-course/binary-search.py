def binary_search(arr, target):
    """
    Performs a binary search on a sorted array to find the index of a target value.
    Args:
        arr (list): A sorted list of comparable elements.
        target: The value to search for in the array.
    Returns:
        int: The index of the target value if found, otherwise -1.
    Time Complexity:
        O(log n) - where n is the length of the array.
    Space Complexity:
        O(1) - uses only constant extra space.
    Example:
        >>> binary_search([1, 3, 5, 7, 9], 5)
        2
        >>> binary_search([1, 3, 5, 7, 9], 4)
        -1
    """
    left, right = 0, len(arr) - 1

    while left <= right:
        mid = (left + right) // 2

        if arr[mid] == target:
            return mid  # found: return index
        elif arr[mid] < target:
            left = mid + 1
        else:
            right = mid - 1

    return -1  # not found